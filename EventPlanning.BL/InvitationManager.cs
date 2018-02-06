using EventPlanning.DAL;
using EventPlanning.Model;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;

namespace EventPlanning.BL
{
    public class InvitationManager
    {
        private EventContext context;
        private Repository repository;
        private ISmtpClient smtpServer;

        public InvitationManager(EventContext context, ISmtpClient smtpClient)
        {
            this.context = context;
            smtpServer = smtpClient;
            repository = new Repository(this.context);
        }

        /// <summary>
        /// Invites to event
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool InviteToEvent(int eventId, UserData user)
        {
            if (repository.GetEvent(eventId) == null)
            {
                return false;
            }

            var sentTo = SendInvitation(eventId, user);

            if (sentTo != string.Empty)
            {
                return CreateInvitation(eventId, user, sentTo);
            }

            return false;
        }

        /// <summary>
        /// Decides what kind of invitation to send
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public string SendInvitation(int eventId, UserData user)
        {
            if (user.Email != null && SendMail(eventId, user))
            {
                return "Email";
            }

            if (user.Id != 0 && SendNotification(eventId, user))
            {
                return "Notification";
            }

            return string.Empty;
        }

        public bool SendNotification(int eventId, UserData user)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates invitation
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool CreateInvitation(int eventId, UserData user, string sentTo)
        {
            if(sentTo != "Email" && sentTo != "Notification")
            {
                return false;
            }

            var invitation = new Invitation()
            {
                EventId = eventId,
                SentTo = sentTo,
                DateSent = DateTime.Now,
                UserId = user.Id,
                Status = "Waiting",
            };

            return repository.AddInvitation(invitation);
        }

        /// <summary>
        /// Sends email with invitation
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool SendMail(int eventId, UserData user)
        {
            var ev = repository.GetEvent(eventId);
            var mailBody = new StringBuilder();
            mailBody.AppendFormat("You have been invited to event named: {0}.", ev.Title).AppendLine();
            mailBody.Append("To confirm participation you must click link below.").AppendLine();

            var acceptLink = Environment.GetEnvironmentVariable("acceptLink");

            if (acceptLink == null)
            {
                return false;
            }

            mailBody.AppendFormat("{0}/{1}/{2}", acceptLink, eventId, user.Id);

            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("eventinvitator@gmail.com");
            mail.Subject = "Invitation to event";
            mail.Body = mailBody.ToString();

            if (string.IsNullOrEmpty(user.Email))
            {
                return false;
            }

            try
            {
                mail.To.Add(user.Email);
                smtpServer.Send(mail);
            }
            catch (Exception ex) when (ex is FormatException || ex is SmtpFailedRecipientsException)
            {
                return false;
            }

            return true;
        }
    }
}

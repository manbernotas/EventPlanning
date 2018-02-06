using System.Net.Mail;

namespace EventPlanning.BL
{
    public class SmtpWrapper : ISmtpClient
    {
        public SmtpClient SmtpClient { get; set; }

        public SmtpWrapper(string host, int port, string smtpUser, string smtpPass, bool enableSsl)
        {
            SmtpClient = new SmtpClient(host, port);
            SmtpClient.Credentials = new System.Net.NetworkCredential(smtpUser, smtpPass);
            SmtpClient.EnableSsl = enableSsl;
        }

        public void Send(MailMessage mailMessage)
        {
            SmtpClient.Send(mailMessage);
        }
    }
}

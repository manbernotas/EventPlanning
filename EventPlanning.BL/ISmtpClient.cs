using System.Net.Mail;

namespace EventPlanning.BL
{
    public interface ISmtpClient
    {
        void Send(MailMessage mailMessage);
    }
}

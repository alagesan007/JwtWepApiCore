using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MailKit;
using User.Management.Service.Models;


namespace User.Management.Service.Services
{
    public class MyEmailService : IMyEmailService
    {
        public readonly MyEmailConfiguration _emailConfig;

        public MyEmailService(MyEmailConfiguration emailconfig) => _emailConfig = emailconfig;
        public void SendEmail(Message message)
        {
            var emailMessage = CreateEmailMessage(message);
            Send(emailMessage);
        }
        private MimeMessage CreateEmailMessage(Message message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(MailboxAddress.Parse(_emailConfig.From));
            emailMessage.To.AddRange(message.To);
            emailMessage.Subject = message.Subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Text) { Text = message.Content };
            
            return emailMessage;
        }
        private void Send(MimeMessage mailMessage)
        {


            using var client = new SmtpClient();


            try
            {
                
                client.Connect(_emailConfig.SmtpServer, _emailConfig.Port,true);
                client.Authenticate(_emailConfig.UserName, _emailConfig.Password);
                client.Send(mailMessage);
            }
            catch
            {
                throw;
            }
            finally
            {
                client.Disconnect(true);
                client.Dispose();

            }
        }

        
    }
}

using Microsoft.Extensions.Options;
using PatientBookingSystem.Application.Interfaces;
using PatientBookingSystem.Infrastructure.Configurations;
using System.Net;
using System.Net.Mail;


namespace PatientBookingSystem.Infrastructure.Services
{
    public class NotificationService : INotificationService
    {
        private readonly EmailSettings _awsSettings;

        public NotificationService(IOptions<EmailSettings> awsSettings)
        {
            _awsSettings = awsSettings.Value;
        }

        public async Task SendEmailAsync(
            string email,
            string subject,
            string message)
        {
            try
            {
                using (var client = new SmtpClient(_awsSettings.Host, _awsSettings.Port))
                {
                    client.Credentials = new NetworkCredential(
                        _awsSettings.Username,
                        _awsSettings.Password
                    );

                    client.EnableSsl = true;

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(_awsSettings.SenderEmail),
                        Subject = subject,
                        Body = message,
                        IsBodyHtml = true
                    };

                    mailMessage.To.Add(email);

                    await client.SendMailAsync(mailMessage);

                    Console.WriteLine("Email Sent Successfully");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SMTP Email Error: {ex.Message}");
            }
        }


        //public async Task SendEmailAsync(string email, string subject, string message)
        //{
        //    var emailMessage = new MimeMessage();
        //    emailMessage.From.Add(new MailboxAddress("HomeCare Nursing Services", _emailSettings.SenderEmail));
        //    emailMessage.To.Add(new MailboxAddress("", email));
        //    emailMessage.Subject = subject;

        //    emailMessage.Body = new TextPart("plain")
        //    {
        //        Text = message
        //    };

        //    using var client = new SmtpClient();

        //    await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.Port, SecureSocketOptions.StartTls);
        //    await client.AuthenticateAsync(_emailSettings.SenderEmail, _emailSettings.Password);
        //    await client.SendAsync(emailMessage);
        //    await client.DisconnectAsync(true);
        //}

        public async Task SendSmsAsync(string phone, string message)
        {
            // For now (FREE mode)
            Console.WriteLine($"SMS to {phone}: {message}");
            //return Task.CompletedTask;
        }

    }
}

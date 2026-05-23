using Microsoft.Extensions.Options;
using PatientBookingSystem.Application.Interfaces;
using PatientBookingSystem.Infrastructure.Configurations;
using SendGrid.Helpers.Mail;
using System.Net;
using System.Net.Mail;


namespace PatientBookingSystem.Infrastructure.Services
{
    public class NotificationService : INotificationService
    {
        private readonly EmailSettings _emailSettings;

        public NotificationService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task SendEmailAsync(
       string email,
       string subject,
       string message)
        {
            try
            {
                using (var client = new SmtpClient(
                    _emailSettings.Host,
                    _emailSettings.Port))
                {
                    client.Credentials = new NetworkCredential(
                        _emailSettings.Username,
                        _emailSettings.Password
                    );

                    client.EnableSsl = true;

                    client.DeliveryMethod = SmtpDeliveryMethod.Network;

                    client.UseDefaultCredentials = false;

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(
                            _emailSettings.SenderEmail,
                            _emailSettings.SenderName
                        ),

                        Subject = subject,

                        Body = message,

                        IsBodyHtml = true
                    };

                    mailMessage.To.Add(email);

                    await client.SendMailAsync(mailMessage);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"Email sending failed: {ex.Message}"
                );
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

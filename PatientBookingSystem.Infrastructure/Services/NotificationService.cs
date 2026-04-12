using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MimeKit;
using PatientBookingSystem.Application.Interfaces;
using PatientBookingSystem.Infrastructure.Configurations;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;

namespace PatientBookingSystem.Infrastructure.Services
{
    public class NotificationService : INotificationService
    {
        private readonly EmailSettings _emailSettings;
        private readonly string _apiKey;
        private readonly string _senderEmail;

        public NotificationService(IOptions<EmailSettings> emailSettings, IConfiguration config)
        {
            _emailSettings = emailSettings.Value;
            //_apiKey = config["SendGridSettings:ApiKey"];
            Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
            _senderEmail = config["SendGridSettings:SenderEmail"];
        }
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var client = new SendGridClient(_apiKey);

            var from = new EmailAddress(_senderEmail, "Patient Booking App");
            var to = new EmailAddress(email);

            var msg = MailHelper.CreateSingleEmail(from, to, subject, message, message);

            var response = await client.SendEmailAsync(msg);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Body.ReadAsStringAsync();
                Console.WriteLine("SendGrid Error: " + error);
            }
        }

        //public async Task SendEmailAsync(string email, string subject, string message)
        //{
        //    var emailMessage = new MimeMessage();
        //    emailMessage.From.Add(new MailboxAddress("Patient Booking App", _emailSettings.SenderEmail));
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

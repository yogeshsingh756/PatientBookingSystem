using System;
using System.Collections.Generic;
using System.Text;

namespace PatientBookingSystem.Application.Interfaces
{
    public interface INotificationService
    {
        Task SendSmsAsync(string phone, string message);
        Task SendEmailAsync(string email, string subject, string message);
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace PatientBookingSystem.Infrastructure.Configurations
{
    public class EmailSettings
    {
        public string SmtpServer { get; set; }
        public int Port { get; set; }
        public string SenderEmail { get; set; }
        public string Password { get; set; }
    }
}

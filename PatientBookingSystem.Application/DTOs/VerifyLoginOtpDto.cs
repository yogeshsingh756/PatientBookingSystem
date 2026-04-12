using System;
using System.Collections.Generic;
using System.Text;

namespace PatientBookingSystem.Application.DTOs
{
    public class VerifyLoginOtpDto
    {
        public string PhoneNumber { get; set; }
        public string Otp { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace PatientBookingSystem.Application.DTOs
{
    public class LoginRequestDto
    {
        public string EmailOrPhone { get; set; }
        public string Password { get; set; }
    }
}

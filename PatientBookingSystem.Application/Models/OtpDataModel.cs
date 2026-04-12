using System;
using System.Collections.Generic;
using System.Text;

namespace PatientBookingSystem.Application.Models
{
    public class OtpDataModel
    {
        public string Email { get; set; }
        public string Otp { get; set; }

        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }

        public string Address { get; set; }
        public string Landmark { get; set; }
        public string HouseNumber { get; set; }
        public string Role { get; set; }
    }
}

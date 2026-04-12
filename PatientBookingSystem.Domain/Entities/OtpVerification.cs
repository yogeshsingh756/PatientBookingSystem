using System;
using System.Collections.Generic;
using System.Text;

namespace PatientBookingSystem.Domain.Entities
{
    public class OtpVerification
    {
        public int Id { get; set; }

        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Otp { get; set; }

        // 🔥 Store user data TEMPORARILY
        public string Name { get; set; }
        public string Password { get; set; }
        public string Address { get; set; }
        public string Landmark { get; set; }
        public string HouseNumber { get; set; }
        public string Role { get; set; }

        public DateTime ExpiryTime { get; set; }
        public bool IsUsed { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}

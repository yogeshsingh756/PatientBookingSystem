using System;
using System.Collections.Generic;
using System.Text;

namespace PatientBookingSystem.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        public string PasswordHash { get; set; }

        public string? Address { get; set; }
        public string? Landmark { get; set; }
        public string? HouseNumber { get; set; }
        public string? PinCode { get; set; }

        public string Role { get; set; }

        public bool IsVerified { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool IsActive { get; set; } = false;
    }
}

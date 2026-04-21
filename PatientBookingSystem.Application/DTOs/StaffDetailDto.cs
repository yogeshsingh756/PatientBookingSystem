using System;
using System.Collections.Generic;
using System.Text;

namespace PatientBookingSystem.Application.DTOs
{
    public class StaffDetailDto
    {
        public int StaffId { get; set; }
        public int UserId { get; set; }

        // 🔹 User Info
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        public string Address { get; set; }
        public string Landmark { get; set; }
        public string HouseNumber { get; set; }
        public string PinCode { get; set; }
        public bool IsActive { get; set; } = false;

        // 🔹 Staff Info
        public int StaffType { get; set; }
        public string Qualification { get; set; }
        public int ExperienceYears { get; set; }
        public string Specialization { get; set; }

        public decimal? ConsultationFee { get; set; }
        public decimal? Salary { get; set; }

        public string LicenseNumber { get; set; }

        public string? ProfileImageUrl { get; set; }
        public bool IsAvailable { get; set; }

        // 🔹 Documents
        public List<string> Documents { get; set; }
    }
}

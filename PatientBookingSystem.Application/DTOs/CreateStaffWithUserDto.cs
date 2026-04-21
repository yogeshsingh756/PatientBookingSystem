using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace PatientBookingSystem.Application.DTOs
{
    public class CreateStaffWithUserDto
    {
        // 🔹 USER FIELDS
        public string Name { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; }
        public string Password { get; set; }

        public string Address { get; set; } = string.Empty;
        public string Landmark { get; set; } = string.Empty;
        public string HouseNumber { get; set; } = string.Empty;
        public string PinCode { get; set; } = string.Empty;

        // 🔹 STAFF FIELDS
        public int StaffType { get; set; }
        public string Qualification { get; set; }
        public int ExperienceYears { get; set; }

        public string? Specialization { get; set; }
        public decimal? ConsultationFee { get; set; }
        public decimal? Salary { get; set; }

        public string? LicenseNumber { get; set; }

        public IFormFile? ProfileImage { get; set; }
        public List<IFormFile>? Documents { get; set; }
    }
}

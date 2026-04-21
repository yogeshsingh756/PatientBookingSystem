using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace PatientBookingSystem.Application.DTOs
{
    public class UpdateStaffWithUserDto
    {
        public int StaffId { get; set; }

        // 🔹 USER
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        public string Address { get; set; }
        public string Landmark { get; set; }
        public string HouseNumber { get; set; }
        public string PinCode { get; set; }

        // 🔹 STAFF
        public int StaffType { get; set; }
        public string Qualification { get; set; }
        public int ExperienceYears { get; set; }

        public string? Specialization { get; set; }
        public decimal? ConsultationFee { get; set; }
        public decimal? Salary { get; set; }

        public string? LicenseNumber { get; set; }

        public IFormFile? ProfileImage { get; set; }

        // Optional new documents
        public List<IFormFile>? Documents { get; set; }
    }
}

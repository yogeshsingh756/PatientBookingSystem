using PatientBookingSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PatientBookingSystem.Domain.Entities
{
    public class Staff
    {
        public int Id { get; set; }

        public int UserId { get; set; }   // FK (required)

        public StaffType StaffType { get; set; }

        public string Qualification { get; set; }
        public int ExperienceYears { get; set; }

        public string? Specialization { get; set; }

        public decimal? ConsultationFee { get; set; }

        public decimal? Salary { get; set; }   // ✅ NEW (future use)

        public bool IsAvailable { get; set; } = true;

        public string? LicenseNumber { get; set; }   // ✅ NEW

        public string? ProfileImageUrl { get; set; }

        public DateTime CreatedAt { get; set; }

        // 🔗 Navigation
        public User User { get; set; }

        // 🔥 OPTIONAL RELATION
        public ICollection<StaffDocument>? Documents { get; set; }
    }
}

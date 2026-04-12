using PatientBookingSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PatientBookingSystem.Domain.Entities
{
    public class PatientStatusHistory
    {
        public int Id { get; set; }

        public int PatientId { get; set; }

        public PatientStatus Status { get; set; }

        public string? Remarks { get; set; }

        public int? UpdatedByUserId { get; set; } // optional

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace PatientBookingSystem.Application.DTOs
{
    public class PatientDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ServiceId { get; set; }
        public int? StaffId { get; set; }

        public DateTime AppointmentDate { get; set; }
        public string SlotTime { get; set; } = null!;
        public int? NoOfDays { get; set; }

        public string? DiseaseName { get; set; }
        public DateTime? DischargeDate { get; set; }
        public string? DoctorPrescription { get; set; }
        public string? DiseaseImageUrl { get; set; }

        public string Status { get; set; } = null!;
    }
}

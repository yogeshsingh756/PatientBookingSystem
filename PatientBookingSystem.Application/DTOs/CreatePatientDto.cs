using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace PatientBookingSystem.Application.DTOs
{
    public class CreatePatientDto
    {
        public int UserId { get; set; }
        public int ServiceId { get; set; }
        public int? StaffId { get; set; }

        public DateTime AppointmentDate { get; set; }
        public string SlotTime { get; set; } = null!;
        public int? NoOfDays { get; set; }

        // Optional
        public string? DiseaseName { get; set; }
        public DateTime? DischargeDate { get; set; }
        public string? DoctorPrescription { get; set; }
        public IFormFile? DiseaseImage { get; set; }
    }
}

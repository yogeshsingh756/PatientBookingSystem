using System;
using System.Collections.Generic;
using System.Text;

namespace PatientBookingSystem.Application.DTOs
{
    public class PatientUserAppointmentDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int? ServiceId { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int? StaffId { get; set; }
        public string StaffName { get; set; } = string.Empty;
        public DateTime AppointmentDate { get; set; }
        public string SlotTime { get; set; } = null!;
        public int? NoOfDays { get; set; }

        public string? DiseaseName { get; set; }
        public DateTime? DischargeDate { get; set; }
        public string? DoctorPrescription { get; set; }
        public string? DiseaseImageUrl { get; set; }

        public string Status { get; set; } = null!;
        public string? Remarks { get; set; }
        public string Address { get; set; } = string.Empty;
        public string Landmark { get; set; } = string.Empty;
        public string HouseNumber { get; set; } = string.Empty;
        public string PinCode { get; set; } = string.Empty; 
        public string PhoneNumber { get; set; } = string.Empty;
    }
}

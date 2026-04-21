using PatientBookingSystem.Domain.Enums;

namespace PatientBookingSystem.Domain.Entities
{
    public class PatientAppointment
    {
        public int Id { get; set; }

        public int UserId { get; set; }   // FK from User
        public int? ServiceId { get; set; }
        public int? StaffId { get; set; }

        public DateTime AppointmentDate { get; set; }
        public string SlotTime { get; set; } = null!;
        public int? NoOfDays { get; set; }

        // Treatment History (Optional)
        public string? DiseaseName { get; set; }
        public DateTime? DischargeDate { get; set; }
        public string? DoctorPrescription { get; set; }
        public string? DiseaseImageUrl { get; set; }

        public PatientStatus Status { get; set; } = PatientStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public User User { get; set; }       
        public Service? Service { get; set; }
        public Staff? Staff { get; set; }
    }
}

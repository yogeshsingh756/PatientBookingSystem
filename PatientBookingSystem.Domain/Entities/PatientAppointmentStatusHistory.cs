using PatientBookingSystem.Domain.Enums;

namespace PatientBookingSystem.Domain.Entities
{
    public class PatientAppointmentStatusHistory
    {
        public int Id { get; set; }

        public int PatientId { get; set; }

        public PatientStatus Status { get; set; }

        public string? Remarks { get; set; }

        public int? UpdatedByUserId { get; set; } // optional

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public PatientAppointment? PatientAppointment { get; set; }
    }
}

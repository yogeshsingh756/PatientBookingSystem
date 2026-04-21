namespace PatientBookingSystem.Domain.Entities
{
    public class StaffAvailability
    {
        public int Id { get; set; }

        public int? StaffId { get; set; }

        public DayOfWeek Day { get; set; }   // Monday, Tuesday...

        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // 🔗 Navigation
        public Staff? Staff { get; set; }
    }
}

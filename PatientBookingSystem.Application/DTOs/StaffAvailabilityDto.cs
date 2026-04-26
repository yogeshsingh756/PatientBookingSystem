using System;
using System.Collections.Generic;
using System.Text;

namespace PatientBookingSystem.Application.DTOs
{
    public class StaffAvailabilityDto
    {
        public int Id { get; set; }
        public int StaffId { get; set; }
        public DayOfWeek Day { get; set; }

        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace PatientBookingSystem.Application.DTOs
{
    public class CreateStaffAvailabilityDto
    {
        public int StaffId { get; set; }
        public DayOfWeek Day { get; set; }

        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}

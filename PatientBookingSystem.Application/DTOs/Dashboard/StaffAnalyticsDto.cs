using System;
using System.Collections.Generic;
using System.Text;

namespace PatientBookingSystem.Application.DTOs.Dashboard
{
    public class StaffAnalyticsDto
    {
        public string? MostBookedStaff { get; set; }

        public string? LeastActiveStaff { get; set; }

        public int BusyStaffToday { get; set; }

        public int StaffWithoutAvailability { get; set; }
    }
}

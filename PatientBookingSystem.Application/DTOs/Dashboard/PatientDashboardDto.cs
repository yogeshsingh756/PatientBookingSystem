using System;
using System.Collections.Generic;
using System.Text;

namespace PatientBookingSystem.Application.DTOs.Dashboard
{
    public class PatientDashboardDto
    {
        public UserSummaryDto UserSummary { get; set; }

        public UpcomingAppointmentDto? UpcomingAppointment { get; set; }

        public List<DashboardServiceDto> Services { get; set; }

        public List<DashboardStaffDto> Staffs { get; set; }

        public List<PatientRecentAppointmentDto> RecentAppointments { get; set; }
        public LastAppointmentDto? LastAppointment { get; set; }
    }
}

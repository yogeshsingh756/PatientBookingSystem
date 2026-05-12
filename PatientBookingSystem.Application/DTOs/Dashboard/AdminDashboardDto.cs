using System;
using System.Collections.Generic;
using System.Text;

namespace PatientBookingSystem.Application.DTOs.Dashboard
{
    public class AdminDashboardDto
    {
        public DashboardOverviewDto Overview { get; set; }

        public AppointmentAnalyticsDto AppointmentAnalytics { get; set; }

        public StaffAnalyticsDto StaffAnalytics { get; set; }

        public ServiceAnalyticsDto ServiceAnalytics { get; set; }

        public List<RecentAppointmentDto> RecentAppointments { get; set; }
    }
}

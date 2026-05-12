using System;
using System.Collections.Generic;
using System.Text;

namespace PatientBookingSystem.Application.DTOs.Dashboard
{
    public class DashboardOverviewDto
    {
        public int TotalPatients { get; set; }

        public int TotalStaff { get; set; }

        public int TotalAppointments { get; set; }

        public int TodayAppointments { get; set; }

        public int TodayApprovedAppointments { get; set; }

        public int TodayPendingAppointments { get; set; }

        public int TodayCompletedAppointments { get; set; }

        public int TodayCancelledAppointments { get; set; }

        public int PendingAppointments { get; set; }

        public int ApprovedAppointments { get; set; }

        public int CompletedAppointments { get; set; }

        public int CancelledAppointments { get; set; }

    }
}

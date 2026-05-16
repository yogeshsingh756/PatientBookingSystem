using System;
using System.Collections.Generic;
using System.Text;

namespace PatientBookingSystem.Application.DTOs.Dashboard
{
    public class UserSummaryDto
    {
        public string Name { get; set; }

        public string? ProfileImage { get; set; }

        public int TotalAppointments { get; set; }

        public int TotalApproved { get; set; }

        public int TotalPending { get; set; }

        public int TotalCompleted { get; set; }

        public int TotalCancelled { get; set; }
    }
}

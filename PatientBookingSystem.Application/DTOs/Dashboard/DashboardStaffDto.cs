using System;
using System.Collections.Generic;
using System.Text;

namespace PatientBookingSystem.Application.DTOs.Dashboard
{
    public class DashboardStaffDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string? Specialization { get; set; }

        public int Experience { get; set; }

        public string? Image { get; set; }
    }
}

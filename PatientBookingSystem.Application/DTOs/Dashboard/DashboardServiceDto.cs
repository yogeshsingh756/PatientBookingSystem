using System;
using System.Collections.Generic;
using System.Text;

namespace PatientBookingSystem.Application.DTOs.Dashboard
{
    public class DashboardServiceDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string? Image { get; set; }

        public string? Description { get; set; }

        public string? Category { get; set; }
    }
}

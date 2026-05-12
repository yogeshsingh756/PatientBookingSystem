using PatientBookingSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PatientBookingSystem.Application.DTOs.Dashboard
{
    public class DashboardFilterDto
    {
        public DashboardFilterType Filter { get; set; } = DashboardFilterType.Today;
    }
}

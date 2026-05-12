using System;
using System.Collections.Generic;
using System.Text;

namespace PatientBookingSystem.Application.DTOs.Dashboard
{
    public class ServiceAnalyticsDto
    {
        public string? MostBookedService { get; set; }

        public string? LeastBookedService { get; set; }

        public List<ServiceBookingDto> ServiceWiseBookings { get; set; }
    }
}

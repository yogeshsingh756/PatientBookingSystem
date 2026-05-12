using System;
using System.Collections.Generic;
using System.Text;

namespace PatientBookingSystem.Application.DTOs.Dashboard
{
    public class AppointmentAnalyticsDto
    {
        public double ConversionRate { get; set; }

        public string? MostBookedDay { get; set; }

        public string? MostBookedSlot { get; set; }

        public List<AppointmentTrendDto> Last7DaysTrend { get; set; }
    }
}

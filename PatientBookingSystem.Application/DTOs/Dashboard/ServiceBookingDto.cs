using System;
using System.Collections.Generic;
using System.Text;

namespace PatientBookingSystem.Application.DTOs.Dashboard
{
    public class ServiceBookingDto
    {
        public string ServiceName { get; set; }

        public int TotalBookings { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace PatientBookingSystem.Application.DTOs.Dashboard
{
    public class AppointmentHistoryDto
    {
        public int Id { get; set; }

        public int AppointmentId { get; set; }

        public int Status { get; set; }

        public string? Remarks { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}

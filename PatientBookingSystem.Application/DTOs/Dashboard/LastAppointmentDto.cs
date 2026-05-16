using System;
using System.Collections.Generic;
using System.Text;

namespace PatientBookingSystem.Application.DTOs.Dashboard
{
    public class LastAppointmentDto
    {
        public int Id { get; set; }

        public string ServiceName { get; set; }

        public string? StaffName { get; set; }

        public DateTime AppointmentDate { get; set; }

        public string SlotTime { get; set; }

        public int Status { get; set; }

        public string? AppointmentAddress { get; set; }
        public List<AppointmentHistoryDto> History { get; set; }
    }
}

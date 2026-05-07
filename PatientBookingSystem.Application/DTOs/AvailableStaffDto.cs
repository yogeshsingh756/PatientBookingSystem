using System;
using System.Collections.Generic;
using System.Text;

namespace PatientBookingSystem.Application.DTOs
{
    public class AvailableStaffDto
    {
        public int StaffId { get; set; }
        public string Name { get; set; }
        public string? Specialization { get; set; }
    }
}

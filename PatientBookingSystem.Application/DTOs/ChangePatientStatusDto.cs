using PatientBookingSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PatientBookingSystem.Application.DTOs
{
    public class ChangePatientStatusDto
    {
        public int Id { get; set; }
        public PatientStatus Status { get; set; }
        public string? Remarks { get; set; }
    }
}

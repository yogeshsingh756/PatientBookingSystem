using System;
using System.Collections.Generic;
using System.Text;

namespace PatientBookingSystem.Application.DTOs
{
    public class UserQueryDto
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Search { get; set; }
    }
}

using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace PatientBookingSystem.Application.DTOs
{
    public class CreateServiceDto
    {
        public string Name { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }

        public IFormFile? Image { get; set; } // file upload
        public string? Icon { get; set; }
    }
}

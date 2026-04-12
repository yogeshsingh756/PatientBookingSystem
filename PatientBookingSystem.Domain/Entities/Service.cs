using System;
using System.Collections.Generic;
using System.Text;

namespace PatientBookingSystem.Domain.Entities
{
    public class Service
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;
        public string Category { get; set; } = null!;
        public string Description { get; set; } = null!;

        public string? ImageUrl { get; set; }
        public string? Icon { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

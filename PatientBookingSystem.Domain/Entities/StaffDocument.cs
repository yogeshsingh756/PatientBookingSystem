using PatientBookingSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace PatientBookingSystem.Domain.Entities
{
    public class StaffDocument
    {
        public int Id { get; set; }

        public int? StaffId { get; set; }   // ✅ NULLABLE (OPTIONAL RELATION)

        public DocumentType DocumentType { get; set; }

        public string FileUrl { get; set; }

        public DateTime UploadedAt { get; set; }

        // 🔗 Navigation (optional)
        public Staff? Staff { get; set; }
    }
}

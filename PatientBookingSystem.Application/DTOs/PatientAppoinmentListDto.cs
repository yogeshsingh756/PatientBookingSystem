using System;
using System.Collections.Generic;
using System.Text;

namespace PatientBookingSystem.Application.DTOs
{
    public class PatientAppoinmentListDto
    {
        public int Id { get; set; }

        public string UserName { get; set; }
        public string PhoneNumber { get; set; }

        public string Address { get; set; }
        public string HouseNumber { get; set; }
        public string PinCode { get; set; }

        public string ServiceName { get; set; }
        public string Category { get; set; }

        public DateTime AppointmentDate { get; set; }
        public string SlotTime { get; set; }

        public string DiseaseName { get; set; }

        public int Status { get; set; }
    }
}

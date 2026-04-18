using System;
using System.Collections.Generic;
using System.Text;

namespace PatientBookingSystem.Application.DTOs
{
    public class UpdateUserDto
    {
        public string Name { get; set; }
        public string PhoneNumber { get; set; }

        public string Address { get; set; }
        public string Landmark { get; set; }
        public string HouseNumber { get; set; }
        public string PinCode { get; set; }
    }
}

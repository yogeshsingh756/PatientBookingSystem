using System;
using System.Collections.Generic;
using System.Text;

namespace PatientBookingSystem.Application.DTOs.Common
{
    public class PaginatedResponse<T>
    {
        public List<T> Data { get; set; }
        public int TotalRecords { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}

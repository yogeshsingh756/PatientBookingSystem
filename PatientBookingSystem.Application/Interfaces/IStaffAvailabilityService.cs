using PatientBookingSystem.Application.DTOs;
using PatientBookingSystem.Application.DTOs.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace PatientBookingSystem.Application.Interfaces
{
    public interface IStaffAvailabilityService
    {
        Task<ApiResponse<string>> CreateAsync(CreateStaffAvailabilityDto dto);
        Task<ApiResponse<List<StaffAvailabilityDto>>> GetByStaffId(int staffId);
    }
}

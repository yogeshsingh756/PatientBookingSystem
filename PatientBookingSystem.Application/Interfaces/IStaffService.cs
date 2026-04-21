using PatientBookingSystem.Application.DTOs;
using PatientBookingSystem.Application.DTOs.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace PatientBookingSystem.Application.Interfaces
{
    public interface IStaffService
    {
        Task<ApiResponse<string>> CreateStaffWithUserAsync(CreateStaffWithUserDto dto);
        Task<ApiResponse<string>> UpdateStaffWithUserAsync(UpdateStaffWithUserDto dto);
        Task<ApiResponse<string>> DeleteAsync(int id);

        Task<ApiResponse<PaginatedResponse<StaffDto>>> GetAllAsync(StaffFilterDto filter);
        Task<ApiResponse<StaffDetailDto>> GetByIdAsync(int id);
    }
}

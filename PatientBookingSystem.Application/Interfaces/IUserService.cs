using PatientBookingSystem.Application.DTOs;
using PatientBookingSystem.Application.DTOs.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace PatientBookingSystem.Application.Interfaces
{
    public interface IUserService
    {
        Task<ApiResponse<PaginatedResponse<UserDto>>> GetAllPatientsAsync(UserQueryDto dto);
        Task<ApiResponse<UserDto>> GetByIdAsync(int id);
        Task<ApiResponse<string>> UpdateAsync(int id, UpdateUserDto dto);
        Task<ApiResponse<string>> DeleteAsync(int id);
    }
}

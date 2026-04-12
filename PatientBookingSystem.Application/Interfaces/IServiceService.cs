using PatientBookingSystem.Application.DTOs;
using PatientBookingSystem.Application.DTOs.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace PatientBookingSystem.Application.Interfaces
{
    public interface IServiceService
    {
        Task<ApiResponse<string>> CreateAsync(CreateServiceDto dto);
        Task<ApiResponse<string>> UpdateAsync(int id, CreateServiceDto dto);
        Task<ApiResponse<string>> DeleteAsync(int id);
        Task<ApiResponse<List<ServiceDto>>> GetAllAsync();
        Task<ApiResponse<ServiceDto>> GetByIdAsync(int id);
    }
}

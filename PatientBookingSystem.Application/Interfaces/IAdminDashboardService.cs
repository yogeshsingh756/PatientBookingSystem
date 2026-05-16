using PatientBookingSystem.Application.DTOs.Common;
using PatientBookingSystem.Application.DTOs.Dashboard;
using System;
using System.Collections.Generic;
using System.Text;

namespace PatientBookingSystem.Application.Interfaces
{
    public interface IAdminDashboardService
    {
        Task<ApiResponse<AdminDashboardDto>> GetDashboardAsync(DashboardFilterDto dto);
    }
}

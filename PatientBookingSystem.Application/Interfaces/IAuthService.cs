using PatientBookingSystem.Application.DTOs;
using PatientBookingSystem.Application.DTOs.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace PatientBookingSystem.Application.Interfaces
{
    public interface IAuthService
    {
        Task<ApiResponse<string>> SendOtpAsync(RegisterRequestDto dto);
        Task<ApiResponse<string>> VerifyOtpAndRegisterAsync(VerifyOtpDto dto);
        Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginRequestDto dto);
        Task<ApiResponse<string>> SendLoginOtpAsync(SendLoginOtpDto dto);
        Task<ApiResponse<LoginResponseDto>> VerifyLoginOtpAsync(VerifyLoginOtpDto dto);

    }
}

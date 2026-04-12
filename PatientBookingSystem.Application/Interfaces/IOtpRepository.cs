using PatientBookingSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PatientBookingSystem.Application.Interfaces
{
    public interface IOtpRepository
    {
        Task SaveOtpAsync(OtpVerification otp);
        Task<OtpVerification?> GetValidOtpAsync(string phoneNumber, string otp);
        Task MarkAsUsedAsync(OtpVerification otp);
    }
}

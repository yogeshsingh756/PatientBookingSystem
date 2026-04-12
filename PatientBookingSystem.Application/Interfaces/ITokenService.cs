using PatientBookingSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PatientBookingSystem.Application.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(User user);
    }
}

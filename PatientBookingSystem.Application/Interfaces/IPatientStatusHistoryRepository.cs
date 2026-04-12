using PatientBookingSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PatientBookingSystem.Application.Interfaces
{
    public interface IPatientStatusHistoryRepository
    {
        Task AddAsync(PatientStatusHistory history);
        Task SaveChangesAsync();
    }
}

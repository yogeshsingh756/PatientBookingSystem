using PatientBookingSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PatientBookingSystem.Application.Interfaces
{
    public interface IPatientRepository
    {
        Task AddAsync(Patient patient);
        Task<List<Patient>> GetAllAsync();
        Task<Patient?> GetByIdAsync(int id);
        Task UpdateAsync(Patient patient);
        Task SaveChangesAsync();
    }
}

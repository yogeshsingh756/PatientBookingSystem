using PatientBookingSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PatientBookingSystem.Application.Interfaces
{
    public interface IPatientRepository
    {
        Task AddAsync(PatientAppointment patient);
        Task<List<PatientAppointment>> GetAllAsync();
        Task<PatientAppointment?> GetByIdAsync(int id);
        Task UpdateAsync(PatientAppointment patient);
        Task SaveChangesAsync();
        IQueryable<PatientAppointment> GetQueryable();
    }
}

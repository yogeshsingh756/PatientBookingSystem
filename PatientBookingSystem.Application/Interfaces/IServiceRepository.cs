using PatientBookingSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PatientBookingSystem.Application.Interfaces
{
    public interface IServiceRepository
    {
        Task AddAsync(Service service);
        Task<Service?> GetByIdAsync(int id);
        Task<List<Service>> GetAllAsync();
        Task UpdateAsync(Service service);
        Task SaveChangesAsync();
    }
}

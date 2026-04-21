using PatientBookingSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PatientBookingSystem.Application.Interfaces
{
    public interface IStaffRepository
    {
        Task AddAsync(Staff staff);
        Task UpdateAsync(Staff staff);

        Task<Staff?> GetByIdAsync(int id);

        IQueryable<Staff> GetQueryable();

        Task SaveChangesAsync();
    }
}

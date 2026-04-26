using PatientBookingSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PatientBookingSystem.Application.Interfaces
{
    public interface IStaffAvailabilityRepository
    {
        Task AddAsync(StaffAvailability entity);
        IQueryable<StaffAvailability> GetQueryable();
        Task SaveChangesAsync();
    }
}

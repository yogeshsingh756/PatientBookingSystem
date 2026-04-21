using PatientBookingSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PatientBookingSystem.Application.Interfaces
{
    public interface IStaffDocumentRepository
    {
        Task AddAsync(StaffDocument doc);

        Task<List<StaffDocument>> GetByStaffIdAsync(int staffId);

        Task SaveChangesAsync();
    }
}

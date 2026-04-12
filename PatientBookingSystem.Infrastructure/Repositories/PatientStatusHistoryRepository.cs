using PatientBookingSystem.Application.Interfaces;
using PatientBookingSystem.Domain.Entities;
using PatientBookingSystem.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace PatientBookingSystem.Infrastructure.Repositories
{
    public class PatientStatusHistoryRepository : IPatientStatusHistoryRepository
    {
        private readonly AppDbContext _context;

        public PatientStatusHistoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(PatientStatusHistory history)
        {
            await _context.PatientStatusHistories.AddAsync(history);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}

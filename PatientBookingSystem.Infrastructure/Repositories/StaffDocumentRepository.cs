using PatientBookingSystem.Application.Interfaces;
using PatientBookingSystem.Domain.Entities;
using PatientBookingSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace PatientBookingSystem.Infrastructure.Repositories
{
    public class StaffDocumentRepository : IStaffDocumentRepository
    {
        private readonly AppDbContext _context;

        public StaffDocumentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(StaffDocument doc)
        {
            await _context.StaffDocuments.AddAsync(doc);
        }

        public async Task<List<StaffDocument>> GetByStaffIdAsync(int staffId)
        {
            return await _context.StaffDocuments
                .Where(x => x.StaffId == staffId)
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}

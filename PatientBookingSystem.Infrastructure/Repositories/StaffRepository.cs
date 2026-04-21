using Microsoft.EntityFrameworkCore;
using PatientBookingSystem.Application.Interfaces;
using PatientBookingSystem.Domain.Entities;
using PatientBookingSystem.Infrastructure.Data;

namespace PatientBookingSystem.Infrastructure.Repositories
{
    public class StaffRepository : IStaffRepository
    {
        private readonly AppDbContext _context;

        public StaffRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Staff staff)
        {
            await _context.Staffs.AddAsync(staff);
        }

        public async Task UpdateAsync(Staff staff)
        {
            _context.Staffs.Update(staff);
            await Task.CompletedTask;
        }

        public async Task<Staff?> GetByIdAsync(int id)
        {
            return await _context.Staffs
                .Include(x => x.User)
                .Include(x => x.Documents)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public IQueryable<Staff> GetQueryable()
        {
            return _context.Staffs
                .Include(x => x.User)
                .Include(x => x.Documents)
                .AsQueryable();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}

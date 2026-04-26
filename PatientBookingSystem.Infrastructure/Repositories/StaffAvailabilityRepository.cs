using PatientBookingSystem.Application.Interfaces;
using PatientBookingSystem.Domain.Entities;
using PatientBookingSystem.Infrastructure.Data;

namespace PatientBookingSystem.Infrastructure.Repositories
{
    public class StaffAvailabilityRepository : IStaffAvailabilityRepository
    {
        private readonly AppDbContext _context;

        public StaffAvailabilityRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(StaffAvailability entity)
        {
            await _context.StaffAvailabilities.AddAsync(entity);
        }

        public IQueryable<StaffAvailability> GetQueryable()
        {
            return _context.StaffAvailabilities.AsQueryable();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}

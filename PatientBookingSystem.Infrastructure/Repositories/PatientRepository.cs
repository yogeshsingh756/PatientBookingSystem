using PatientBookingSystem.Application.Interfaces;
using PatientBookingSystem.Domain.Entities;
using PatientBookingSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace PatientBookingSystem.Infrastructure.Repositories
{
    public class PatientRepository : IPatientRepository
    {
        private readonly AppDbContext _context;

        public PatientRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(PatientAppointment patient)
        {
            await _context.PatientAppointments.AddAsync(patient);
        }

        public async Task<List<PatientAppointment>> GetAllAsync()
        {
            return await _context.PatientAppointments.ToListAsync();
        }

        public async Task<PatientAppointment?> GetByIdAsync(int id)
        {
            return await _context.PatientAppointments.FindAsync(id);
        }

        public async Task UpdateAsync(PatientAppointment patient)
        {
            _context.PatientAppointments.Update(patient);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
        public IQueryable<PatientAppointment> GetQueryable()
        {
            return _context.PatientAppointments
                .Include(x => x.User)
                .Include(x => x.Service)
                .AsQueryable();
        }
    }
}

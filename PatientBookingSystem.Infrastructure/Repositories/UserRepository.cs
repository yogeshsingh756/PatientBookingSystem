using PatientBookingSystem.Application.Interfaces;
using PatientBookingSystem.Domain.Entities;
using PatientBookingSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace PatientBookingSystem.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        // ✅ Check if user exists
        public async Task<bool> ExistsAsync(string email, string phone)
        {
            return await _context.Users
                .AnyAsync(u => u.Email == email || u.PhoneNumber == phone);
        }

        // ✅ Add new user
        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        // ✅ Get user by email (for login later)
        public async Task<User?> GetByEmailPhoneAsync(string input)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == input || u.PhoneNumber == input);
        }

        // ✅ GET QUERYABLE (FOR PAGINATION & SEARCH)
        public IQueryable<User> GetQueryable()
        {
            return _context.Users.AsQueryable();
        }

        // ✅ GET BY ID
        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        // ✅ UPDATE USER
        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await Task.CompletedTask; // no DB call here
        }

        // ✅ SAVE CHANGES
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

    }
}

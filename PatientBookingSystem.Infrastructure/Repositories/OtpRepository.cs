using PatientBookingSystem.Application.Interfaces;
using PatientBookingSystem.Domain.Entities;
using PatientBookingSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace PatientBookingSystem.Infrastructure.Repositories
{
    public class OtpRepository : IOtpRepository
    {
        private readonly AppDbContext _context;

        public OtpRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task SaveOtpAsync(OtpVerification otp)
        {
            await _context.OtpVerifications.AddAsync(otp);
            await _context.SaveChangesAsync();
        }

        public async Task<OtpVerification?> GetValidOtpAsync(string phoneNumber, string otp)
        {
            return await _context.OtpVerifications
                .Where(x => x.PhoneNumber == phoneNumber
                         && x.Otp == otp
                         && !x.IsUsed
                         && x.ExpiryTime > DateTime.UtcNow)
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task MarkAsUsedAsync(OtpVerification otp)
        {
            otp.IsUsed = true;
            _context.OtpVerifications.Update(otp);
            await _context.SaveChangesAsync();
        }
    }
}

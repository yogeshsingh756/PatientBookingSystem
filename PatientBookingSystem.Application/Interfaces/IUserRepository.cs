using PatientBookingSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PatientBookingSystem.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<bool> ExistsAsync(string email, string phone);
        Task AddAsync(User user);
        Task<User?> GetByEmailPhoneAsync(string input);
        IQueryable<User> GetQueryable();
        Task<User?> GetByIdAsync(int id);
        Task UpdateAsync(User user);
        Task SaveChangesAsync();
    }
}

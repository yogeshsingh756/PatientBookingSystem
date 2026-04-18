using PatientBookingSystem.Application.DTOs;
using PatientBookingSystem.Application.DTOs.Common;
using PatientBookingSystem.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace PatientBookingSystem.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repo;

        public UserService(IUserRepository repo)
        {
            _repo = repo;
        }

        // ✅ GET ALL WITH PAGINATION + SEARCH
        public async Task<ApiResponse<PaginatedResponse<UserDto>>> GetAllPatientsAsync(UserQueryDto dto)
        {
            var query = _repo.GetQueryable()
                             .Where(x => x.Role == "Patient"); // 👈 only patient

            // 🔍 SEARCH
            if (!string.IsNullOrEmpty(dto.Search))
            {
                query = query.Where(x =>
                    x.Name.Contains(dto.Search) ||
                    x.Email.Contains(dto.Search) ||
                    x.PhoneNumber.Contains(dto.Search));
            }

            // 📊 TOTAL COUNT
            var totalRecords = query.Count();

            // 📄 PAGINATION
            var data = query
                .OrderByDescending(x => x.Id)
                .Skip((dto.PageNumber - 1) * dto.PageSize)
                .Take(dto.PageSize)
                .Select(x => new UserDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Email = x.Email,
                    PhoneNumber = x.PhoneNumber,
                    Address = x.Address,
                    Landmark = x.Landmark,
                    HouseNumber = x.HouseNumber,
                    PinCode = x.PinCode,
                    Role = x.Role,
                    IsVerified = x.IsVerified,
                    IsActive = x.IsActive,
                    CreatedAt = x.CreatedAt
                })
                .ToList();

            return ApiResponse<PaginatedResponse<UserDto>>.SuccessResponse(
                new PaginatedResponse<UserDto>
                {
                    Data = data,
                    TotalRecords = totalRecords,
                    PageNumber = dto.PageNumber,
                    PageSize = dto.PageSize
                },
                "Patients fetched successfully"
            );
        }

        // ✅ GET BY ID
        public async Task<ApiResponse<UserDto>> GetByIdAsync(int id)
        {
            var user = await _repo.GetByIdAsync(id);

            if (user == null || user.Role != "Patient")
                return ApiResponse<UserDto>.FailResponse("User not found");

            var result = new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                Landmark = user.Landmark,
                HouseNumber = user.HouseNumber,
                PinCode = user.PinCode,
                Role = user.Role,
                IsVerified = user.IsVerified,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt
            };

            return ApiResponse<UserDto>.SuccessResponse(result);
        }

        // ✅ UPDATE
        public async Task<ApiResponse<string>> UpdateAsync(int id, UpdateUserDto dto)
        {
            var user = await _repo.GetByIdAsync(id);

            if (user == null || user.Role != "Patient")
                return ApiResponse<string>.FailResponse("User not found");

            user.Name = dto.Name;
            user.PhoneNumber = dto.PhoneNumber;

            user.Address = dto.Address;
            user.Landmark = dto.Landmark;
            user.HouseNumber = dto.HouseNumber;
            user.PinCode = dto.PinCode;

            await _repo.UpdateAsync(user);
            await _repo.SaveChangesAsync();

            return ApiResponse<string>.SuccessResponse("User updated successfully");
        }

        // ✅ DELETE (SOFT DELETE)
        public async Task<ApiResponse<string>> DeleteAsync(int id)
        {
            var user = await _repo.GetByIdAsync(id);

            if (user == null || user.Role != "Patient")
                return ApiResponse<string>.FailResponse("User not found");

            user.IsActive = false;

            await _repo.UpdateAsync(user);
            await _repo.SaveChangesAsync();

            return ApiResponse<string>.SuccessResponse("User deleted successfully");
        }
    }
}

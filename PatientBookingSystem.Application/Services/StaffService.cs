using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PatientBookingSystem.Application.DTOs;
using PatientBookingSystem.Application.DTOs.Common;
using PatientBookingSystem.Application.Interfaces;
using PatientBookingSystem.Domain.Entities;
using PatientBookingSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PatientBookingSystem.Application.Services
{
    public class StaffService : IStaffService
    {
        private readonly IStaffRepository _repo;
        private readonly IStaffDocumentRepository _docRepo;
        private readonly IUserRepository _userRepo;
        private readonly IHttpContextAccessor _http;
        private readonly IUnitOfWork _unitOfWork;

        public StaffService(IStaffRepository repo, IStaffDocumentRepository docRepo, IUserRepository userRepo, IHttpContextAccessor http, IUnitOfWork unitOfWork)
        {
            _repo = repo;
            _docRepo = docRepo;
            _userRepo = userRepo;
            _http = http;
            _unitOfWork = unitOfWork;
        }

        // ✅ CREATE
        public async Task<ApiResponse<string>> CreateStaffWithUserAsync(CreateStaffWithUserDto dto)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var exists = await _userRepo.ExistsAsync(dto.Email, dto.PhoneNumber);
                if (exists)
                    return ApiResponse<string>.FailResponse("User already exists");

                var user = new User
                {
                    Name = dto.Name,
                    Email = dto.Email,
                    PhoneNumber = dto.PhoneNumber,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),

                    Address = dto.Address,
                    Landmark = dto.Landmark,
                    HouseNumber = dto.HouseNumber,
                    PinCode = dto.PinCode,

                    Role = "Staff",
                    IsVerified = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                await _userRepo.AddAsync(user);

                var staff = new Staff
                {
                    UserId = user.Id,
                    StaffType = (StaffType)dto.StaffType,
                    Qualification = dto.Qualification,
                    ExperienceYears = dto.ExperienceYears,
                    Specialization = dto.Specialization,
                    ConsultationFee = dto.ConsultationFee,
                    Salary = dto.Salary,
                    LicenseNumber = dto.LicenseNumber,
                    CreatedAt = DateTime.UtcNow
                };

                if (dto.ProfileImage != null)
                    staff.ProfileImageUrl = await SaveFile(dto.ProfileImage, "staff");

                await _repo.AddAsync(staff);
                await _unitOfWork.SaveChangesAsync();
                if (dto.Documents != null)
                {
                    foreach (var file in dto.Documents)
                    {
                        var url = await SaveFile(file, "staff");

                        await _docRepo.AddAsync(new StaffDocument
                        {
                            StaffId = staff.Id,
                            FileUrl = url,
                            DocumentType = DocumentType.Other,
                            UploadedAt = DateTime.UtcNow
                        });
                    }
                }

                // ✅ SINGLE SAVE
                await _unitOfWork.CommitAsync();

                return ApiResponse<string>.SuccessResponse("Staff created successfully");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return ApiResponse<string>.FailResponse(ex.Message);
            }
        }

        // ✅ GET ALL (PAGINATION + SEARCH)
        public async Task<ApiResponse<PaginatedResponse<StaffDto>>> GetAllAsync(StaffFilterDto filter)
        {
            var query = _repo.GetQueryable();

            if (!string.IsNullOrEmpty(filter.Search))
            {
                query = query.Where(x =>
                    x.User.Name.Contains(filter.Search) ||
                    x.User.Email.Contains(filter.Search) ||
                    x.User.PhoneNumber.Contains(filter.Search));
            }

            var total = await query.CountAsync();

            var data = await query
                .Include(x => x.User)
                .Include(x => x.Documents)
                .OrderByDescending(x => x.Id)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            var baseUrl = $"{_http.HttpContext.Request.Scheme}://{_http.HttpContext.Request.Host}";

            var result = data.Select(x => new StaffDto
            {
                Id = x.Id,
                UserId = x.UserId,
                UserName = x.User.Name,
                Email = x.User.Email,
                Phone = x.User.PhoneNumber,
                Address = x.User.Address,
                HouseNumber = x.User.HouseNumber,
                Landmark = x.User.Landmark,
                PinCode = x.User.PinCode,
                IsActive = x.User.IsActive,

                StaffType = (int)x.StaffType,
                Qualification = x.Qualification,
                ExperienceYears = x.ExperienceYears,

                Specialization = x.Specialization,
                ConsultationFee = x.ConsultationFee,
                Salary = x.Salary,
                LicenseNumber = x.LicenseNumber,
               IsAvailable = x.IsAvailable,

                ProfileImageUrl = x.ProfileImageUrl != null ? baseUrl + x.ProfileImageUrl : null,

                Documents = x.Documents != null
                    ? x.Documents.Select(d => baseUrl + d.FileUrl).ToList()
                    : new List<string>()
            }).ToList();

            return ApiResponse<PaginatedResponse<StaffDto>>.SuccessResponse(
    new PaginatedResponse<StaffDto>(result, total, filter.PageNumber, filter.PageSize),
    "Staff fetched successfully"
);
        }

        // ✅ GET BY ID
        public async Task<ApiResponse<StaffDetailDto>> GetByIdAsync(int id)
        {
            var staff = await _repo.GetQueryable()
                .Include(x => x.User)
                .Include(x => x.Documents)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (staff == null)
                return ApiResponse<StaffDetailDto>.FailResponse("Staff not found");

            var baseUrl = $"{_http.HttpContext.Request.Scheme}://{_http.HttpContext.Request.Host}";

            var result = new StaffDetailDto
            {
                StaffId = staff.Id,
                UserId = staff.UserId,

                Name = staff.User?.Name,
                Email = staff.User?.Email,
                PhoneNumber = staff.User?.PhoneNumber,

                Address = staff.User?.Address,
                Landmark = staff.User?.Landmark,
                HouseNumber = staff.User?.HouseNumber,
                PinCode = staff.User?.PinCode,
                IsActive = staff.User?.IsActive ?? false,

                StaffType = (int)staff.StaffType,
                Qualification = staff.Qualification,
                ExperienceYears = staff.ExperienceYears,
                Specialization = staff.Specialization,

                ConsultationFee = staff.ConsultationFee,
                Salary = staff.Salary,
                LicenseNumber = staff.LicenseNumber,
                IsAvailable = staff.IsAvailable,

                ProfileImageUrl = staff.ProfileImageUrl != null
                    ? baseUrl + staff.ProfileImageUrl
                    : null,

                Documents = staff.Documents != null
                    ? staff.Documents.Select(d => baseUrl + d.FileUrl).ToList()
                    : new List<string>()
            };

            return ApiResponse<StaffDetailDto>.SuccessResponse(result, "Staff Fetched successfully");
        }

        // ✅ UPDATE
        public async Task<ApiResponse<string>> UpdateStaffWithUserAsync(UpdateStaffWithUserDto dto)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var staff = await _repo.GetByIdAsync(dto.StaffId);
                if (staff == null)
                    return ApiResponse<string>.FailResponse("Staff not found");

                var user = await _userRepo.GetByIdAsync(staff.UserId);
                if (user == null)
                    return ApiResponse<string>.FailResponse("User not found");

                // 🔹 USER UPDATE
                user.Name = dto.Name;
                user.Email = dto.Email;
                user.PhoneNumber = dto.PhoneNumber;
                user.Address = dto.Address;
                user.Landmark = dto.Landmark;
                user.HouseNumber = dto.HouseNumber;
                user.PinCode = dto.PinCode;
                user.IsActive = dto.IsActive;

                await _userRepo.UpdateAsync(user);

                // 🔹 STAFF UPDATE
                staff.StaffType = (StaffType)dto.StaffType;
                staff.Qualification = dto.Qualification;
                staff.ExperienceYears = dto.ExperienceYears;
                staff.Specialization = dto.Specialization;
                staff.ConsultationFee = dto.ConsultationFee;
                staff.Salary = dto.Salary;
                staff.LicenseNumber = dto.LicenseNumber;
                staff.IsAvailable = dto.IsAvailable;

                if (dto.ProfileImage != null)
                    staff.ProfileImageUrl = await SaveFile(dto.ProfileImage, "staff");

                await _repo.UpdateAsync(staff);

                // 🔹 DOCUMENTS
                if (dto.Documents != null)
                {
                    foreach (var file in dto.Documents)
                    {
                        var url = await SaveFile(file, "staff");

                        await _docRepo.AddAsync(new StaffDocument
                        {
                            StaffId = staff.Id,
                            FileUrl = url,
                            DocumentType = DocumentType.Other,
                            UploadedAt = DateTime.UtcNow
                        });
                    }
                }

                // ✅ SINGLE SAVE
                await _unitOfWork.CommitAsync();

                return ApiResponse<string>.SuccessResponse("Staff updated successfully");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return ApiResponse<string>.FailResponse(ex.Message);
            }
        }

        // ✅ DELETE (SOFT)
        public async Task<ApiResponse<string>> DeleteAsync(int id)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var staff = await _repo.GetByIdAsync(id);
                if (staff == null)
                    return ApiResponse<string>.FailResponse("Staff not found");

                // 🔹 Get related user
                var user = await _userRepo.GetByIdAsync(staff.UserId);
                if (user == null)
                    return ApiResponse<string>.FailResponse("User not found");

                // 🔹 Soft delete
                staff.IsAvailable = false;
                user.IsActive = false;

                await _repo.UpdateAsync(staff);
                await _userRepo.UpdateAsync(user);

                await _unitOfWork.CommitAsync();

                return ApiResponse<string>.SuccessResponse("Staff deleted successfully");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return ApiResponse<string>.FailResponse(ex.Message);
            }
        }

        // 🔥 FILE SAVE
        private async Task<string> SaveFile(IFormFile file, string folderName)
        {
            var folder = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot/uploads/{folderName}");

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var path = Path.Combine(folder, fileName);

            using var stream = new FileStream(path, FileMode.Create);
            await file.CopyToAsync(stream);

            return $"/uploads/{folderName}/{fileName}";
        }
    }
}

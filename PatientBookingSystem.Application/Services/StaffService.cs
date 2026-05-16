using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PatientBookingSystem.Application.DTOs;
using PatientBookingSystem.Application.DTOs.Common;
using PatientBookingSystem.Application.Interfaces;
using PatientBookingSystem.Domain.Entities;
using PatientBookingSystem.Domain.Enums;
using System.Linq;

namespace PatientBookingSystem.Application.Services
{
    public class StaffService : IStaffService
    {
        private readonly IStaffRepository _repo;
        private readonly IStaffDocumentRepository _docRepo;
        private readonly IUserRepository _userRepo;
        private readonly IHttpContextAccessor _http;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStaffAvailabilityRepository _availabilityRepo;
        private readonly IPatientRepository _appointmentRepo;
        private readonly INotificationService _notificationService;

        public StaffService(IStaffRepository repo, IStaffDocumentRepository docRepo, IUserRepository userRepo, IHttpContextAccessor http, IUnitOfWork unitOfWork, IStaffAvailabilityRepository availabilityRepo,IPatientRepository patientRepository, INotificationService notificationService)
        {
            _repo = repo;
            _docRepo = docRepo;
            _userRepo = userRepo;
            _http = http;
            _unitOfWork = unitOfWork;
            _availabilityRepo = availabilityRepo;
            _appointmentRepo = patientRepository;
            _notificationService = notificationService;
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
                var rawPassword = dto.Password;
                var user = new User
                {
                    Name = dto.Name,
                    Email = dto.Email ?? string.Empty,
                    PhoneNumber = dto.PhoneNumber,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),

                    Address = dto.Address ?? string.Empty,
                    Landmark = dto.Landmark ?? string.Empty,
                    HouseNumber = dto.HouseNumber ?? string.Empty,
                    PinCode = dto.PinCode ?? string.Empty,

                    Role = "Staff",
                    IsVerified = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    Gender = dto.Gender ?? string.Empty,
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
                await SendStaffWelcomeNotification(user, rawPassword);

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
                Gender = x.User.Gender,

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
                Gender = staff.User?.Gender,

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
                user.Email = dto.Email ?? string.Empty;
                user.PhoneNumber = dto.PhoneNumber;
                user.Address = dto.Address ?? string.Empty;
                user.Landmark = dto.Landmark ?? string.Empty;
                user.HouseNumber = dto.HouseNumber ?? string.Empty;
                user.PinCode = dto.PinCode ?? string.Empty;
                user.IsActive = dto.IsActive;
                user.Gender = dto.Gender ?? string.Empty;

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

        public async Task<ApiResponse<List<AvailableStaffDto>>> GetAvailableStaff(
    DateTime date,
    string slot)
        {
            var day = date.DayOfWeek;

            var timeParts = slot.Split('-');
            var start = TimeSpan.Parse(timeParts[0]);
            var end = TimeSpan.Parse(timeParts[1]);

            // 1️⃣ Get staff with availability
            var availableStaff = await _availabilityRepo.GetQueryable()
                .Where(a =>
                    a.Day == day &&
                    a.IsActive &&
                    a.StartTime <= start &&
                    a.EndTime >= end
                )
                .Select(a => a.StaffId)
                .Distinct()
                .ToListAsync();

            // 2️⃣ Remove already booked staff
            var bookedStaff = await _appointmentRepo.GetQueryable()
                .Where(x =>
                    x.AppointmentDate.Date == date.Date &&
                    x.SlotTime == slot &&
                    x.Status == PatientStatus.Approved
                )
                .Select(x => x.StaffId)
                .ToListAsync();

            var bookedSet = bookedStaff.ToHashSet();

            var finalStaffIds = availableStaff
                .Where(id => !bookedSet.Contains(id))
                .ToList();

            // 3️⃣ Get staff details
            var staffList = await _repo.GetQueryable()
                .Include(s => s.User)
                .Where(s => finalStaffIds.Contains(s.Id) && s.IsAvailable)
                .ToListAsync();

            // 4️⃣ Map
            var result = staffList.Select(s => new AvailableStaffDto
            {
                StaffId = s.Id,
                Name = s.User.Name,
                Specialization = s.Specialization
            }).ToList();

            return ApiResponse<List<AvailableStaffDto>>.SuccessResponse(result, "Available staff fetched");
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
        private async Task SendStaffWelcomeNotification(
    User user,
    string password)
        {
            try
            {
                var loginMessage = $@"
            <h2>Welcome to HomeCare Nursing Services</h2>

            <p>Hello {user.Name},</p>

            <p>Your staff account has been created successfully.</p>

            <p>
                <b>Login Email:</b> {user.Email}<br/>
                <b>Phone Number:</b> {user.PhoneNumber}<br/>
                <b>Password:</b> {password}
            </p>

            <p>
                You can login using either your email address or phone number.
            </p>

            <p>
                Please change your password after first login.
            </p>

            <p>Thank you.</p>
        ";

                // ================= EMAIL =================

                if (!string.IsNullOrWhiteSpace(user.Email))
                {
                    await _notificationService.SendEmailAsync(
                        user.Email,
                        "Welcome to HomeCare Nursing Services",
                        loginMessage);
                }

                // ================= SMS =================

                if (!string.IsNullOrWhiteSpace(user.PhoneNumber))
                {
                    var smsMessage =
                        $"Welcome to HomeCare Nursing Services. " +
                        $"Login Phone: {user.PhoneNumber}, " +
                        $"Password: {password}";

                    //await _notificationService.SendSmsAsync(
                    //    user.PhoneNumber,
                    //    smsMessage);
                }
            }
            catch
            {
                // Ignore notification failure
            }
        }
    }
}

using Microsoft.AspNetCore.Http;
using PatientBookingSystem.Application.DTOs.Common;
using PatientBookingSystem.Application.DTOs.Dashboard;
using PatientBookingSystem.Application.Interfaces;
using PatientBookingSystem.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace PatientBookingSystem.Application.Services
{
    public class PatientDashboardService : IPatientDashboardService
    {
        private readonly IUserRepository _userRepo;
        private readonly IPatientRepository _appointmentRepo;
        private readonly IServiceRepository _serviceRepo;
        private readonly IStaffRepository _staffRepo;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPatientStatusHistoryRepository _historyRepo;

        public PatientDashboardService(
            IUserRepository userRepo,
            IPatientRepository appointmentRepo,
            IServiceRepository serviceRepo,
            IStaffRepository staffRepo,
            IHttpContextAccessor httpContextAccessor,
            IPatientStatusHistoryRepository historyRepo )
        {
            _userRepo = userRepo;
            _appointmentRepo = appointmentRepo;
            _serviceRepo = serviceRepo;
            _staffRepo = staffRepo;
            _httpContextAccessor = httpContextAccessor;
            _historyRepo = historyRepo;
        }

        public async Task<ApiResponse<PatientDashboardDto>> GetDashboardAsync()
        {
            try
            {
                // ================= USER ID FROM JWT =================

                var userIdClaim = _httpContextAccessor.HttpContext?
                    .User?
                    .Claims?
                    .FirstOrDefault(x => x.Type == "UserId");

                if (userIdClaim == null)
                {
                    return ApiResponse<PatientDashboardDto>
                        .FailResponse("Unauthorized");
                }

                int userId = Convert.ToInt32(userIdClaim.Value);

                var baseUrl =
                    $"{_httpContextAccessor.HttpContext.Request.Scheme}://" +
                    $"{_httpContextAccessor.HttpContext.Request.Host}";

                // ================= USER =================

                var user = await _userRepo.GetByIdAsync(userId);

                if (user == null)
                {
                    return ApiResponse<PatientDashboardDto>
                        .FailResponse("User not found");
                }

                // ================= USER APPOINTMENTS QUERY =================

                var userAppointments = _appointmentRepo.GetQueryable()
                    .Where(x => x.UserId == userId);

                // ================= COUNTS =================

                var totalAppointments = await userAppointments.CountAsync();

                var totalApproved = await userAppointments
                    .CountAsync(x => x.Status == PatientStatus.Approved);

                var totalPending = await userAppointments
                    .CountAsync(x => x.Status == PatientStatus.Pending);

                var totalCompleted = await userAppointments
                    .CountAsync(x => x.Status == PatientStatus.Completed);

                var totalCancelled = await userAppointments
                    .CountAsync(x => x.Status == PatientStatus.Cancelled);

                // ================= UPCOMING APPOINTMENT =================

                var upcoming = await userAppointments
                    .Include(x => x.Service)
                    .Include(x => x.Staff)
                        .ThenInclude(s => s.User)
                    .Where(x =>
                        x.AppointmentDate.Date >= DateTime.UtcNow.Date &&
                        x.Status != PatientStatus.Cancelled)
                    .OrderBy(x => x.AppointmentDate)
                    .FirstOrDefaultAsync();

                UpcomingAppointmentDto? upcomingDto = null;

                if (upcoming != null)
                {
                    upcomingDto = new UpcomingAppointmentDto
                    {
                        Id = upcoming.Id,

                        ServiceName = upcoming.Service != null
                            ? upcoming.Service.Name
                            : "",

                        StaffAssigned = upcoming.StaffId != null,

                        StaffName = upcoming.Staff != null
                            ? upcoming.Staff.User.Name
                            : null,

                        StaffImage = upcoming.Staff?.ProfileImageUrl != null
                            ? baseUrl + upcoming.Staff.ProfileImageUrl
                            : null,

                        AppointmentDate = upcoming.AppointmentDate,

                        SlotTime = upcoming.SlotTime,

                        Status = (int)upcoming.Status,

                        AppointmentAddress = upcoming.AppointmentAddress
                    };
                }

                // ================= ACTIVE SERVICES =================

                var services = await _serviceRepo.GetQueryable()
                    .Where(x => x.IsActive)
                    .OrderByDescending(x => x.Id)
                    .Take(10)
                    .Select(x => new DashboardServiceDto
                    {
                        Id = x.Id,

                        Name = x.Name,

                        Image = x.ImageUrl != null
                            ? baseUrl + x.ImageUrl
                            : null,

                        Description = x.Description,

                        Category = x.Category
                    })
                    .ToListAsync();

                // ================= ACTIVE STAFF =================

                var staffs = await _staffRepo.GetQueryable()
                    .Include(x => x.User)
                    .Where(x => x.IsAvailable)
                    .OrderByDescending(x => x.Id)
                    .Take(10)
                    .Select(x => new DashboardStaffDto
                    {
                        Id = x.Id,

                        Name = x.User.Name,

                        Specialization = x.Specialization,

                        Experience = x.ExperienceYears,

                        Image = x.ProfileImageUrl != null
                            ? baseUrl + x.ProfileImageUrl
                            : null
                    })
                    .ToListAsync();

                // ================= RECENT APPOINTMENTS =================

                var recentAppointments = await userAppointments
                    .Include(x => x.Service)
                    .Include(x => x.Staff)
                        .ThenInclude(s => s.User)
                    .OrderByDescending(x => x.Id)
                    .Take(5)
                    .Select(x => new PatientRecentAppointmentDto
                    {
                        Id = x.Id,

                        ServiceName = x.Service != null
                            ? x.Service.Name
                            : "",

                        StaffName = x.Staff != null
                            ? x.Staff.User.Name
                            : null,

                        AppointmentDate = x.AppointmentDate,

                        SlotTime = x.SlotTime,

                        Status = (int)x.Status,

                        History = new List<AppointmentHistoryDto>()
                    })
                    .ToListAsync();

                // ================= HISTORY FOR RECENT APPOINTMENTS =================

                var appointmentIds = recentAppointments
                    .Select(x => x.Id)
                    .ToList();

                var histories = await _historyRepo.GetQueryable()
                    .Where(x => appointmentIds.Contains(x.PatientId))
                    .OrderByDescending(x => x.Id)
                    .Select(x => new
                    {
                        x.PatientId,

                        History = new AppointmentHistoryDto
                        {
                            Id = x.Id,

                            AppointmentId = x.PatientId,

                            Status = (int)x.Status,

                            Remarks = x.Remarks,

                            CreatedAt = x.CreatedAt
                        }
                    })
                    .ToListAsync();

                foreach (var appointment in recentAppointments)
                {
                    appointment.History = histories
                        .Where(x => x.PatientId == appointment.Id)
                        .Select(x => x.History)
                        .ToList();
                }

                // ================= LAST APPOINTMENT =================

                var lastAppointment = await userAppointments
                    .Include(x => x.Service)
                    .Include(x => x.Staff)
                        .ThenInclude(s => s.User)
                    .OrderByDescending(x => x.Id)
                    .FirstOrDefaultAsync();

                LastAppointmentDto? lastAppointmentDto = null;

                if (lastAppointment != null)
                {
                    lastAppointmentDto = new LastAppointmentDto
                    {
                        Id = lastAppointment.Id,

                        ServiceName = lastAppointment.Service != null
                            ? lastAppointment.Service.Name
                            : "",

                        StaffName = lastAppointment.Staff != null
                            ? lastAppointment.Staff.User.Name
                            : null,

                        AppointmentDate = lastAppointment.AppointmentDate,

                        SlotTime = lastAppointment.SlotTime,

                        Status = (int)lastAppointment.Status,

                        AppointmentAddress = lastAppointment.AppointmentAddress,

                        History = new List<AppointmentHistoryDto>()
                    };

                    // ================= LAST APPOINTMENT HISTORY =================

                    var lastAppointmentHistory = await _historyRepo.GetQueryable()
                        .Where(x => x.PatientId == lastAppointment.Id)
                        .OrderByDescending(x => x.Id)
                        .Select(x => new AppointmentHistoryDto
                        {
                            Id = x.Id,

                            AppointmentId = x.PatientId,

                            Status = (int)x.Status,

                            Remarks = x.Remarks,

                            CreatedAt = x.CreatedAt
                        })
                        .ToListAsync();

                    lastAppointmentDto.History = lastAppointmentHistory;
                }

                // ================= FINAL RESPONSE =================

                var result = new PatientDashboardDto
                {
                    UserSummary = new UserSummaryDto
                    {
                        Name = user.Name,

                        ProfileImage = null,

                        TotalAppointments = totalAppointments,

                        TotalApproved = totalApproved,

                        TotalPending = totalPending,

                        TotalCompleted = totalCompleted,

                        TotalCancelled = totalCancelled
                    },

                    UpcomingAppointment = upcomingDto,

                    LastAppointment = lastAppointmentDto,

                    Services = services,

                    Staffs = staffs,

                    RecentAppointments = recentAppointments
                };

                return ApiResponse<PatientDashboardDto>
                    .SuccessResponse(result, "Dashboard fetched successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<PatientDashboardDto>
                    .FailResponse(ex.Message);
            }
        }
    }
}


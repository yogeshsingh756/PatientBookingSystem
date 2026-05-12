using PatientBookingSystem.Application.DTOs.Common;
using PatientBookingSystem.Application.DTOs.Dashboard;
using PatientBookingSystem.Application.Interfaces;
using PatientBookingSystem.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace PatientBookingSystem.Application.Services
{
    public class AdminDashboardService : IAdminDashboardService
    {
        private readonly IUserRepository _userRepo;
        private readonly IStaffRepository _staffRepo;
        private readonly IPatientRepository _appointmentRepo;
        private readonly IServiceRepository _serviceRepo;
        private readonly IStaffAvailabilityRepository _availabilityRepo;

        public AdminDashboardService(
            IUserRepository userRepo,
            IStaffRepository staffRepo,
            IPatientRepository appointmentRepo,
            IServiceRepository serviceRepo,
            IStaffAvailabilityRepository availabilityRepo)
        {
            _userRepo = userRepo;
            _staffRepo = staffRepo;
            _appointmentRepo = appointmentRepo;
            _serviceRepo = serviceRepo;
            _availabilityRepo = availabilityRepo;
        }

        public async Task<ApiResponse<AdminDashboardDto>> GetDashboardAsync()
        {
            var today = DateTime.UtcNow.Date;

            // ================= OVERVIEW =================

            var totalPatients = await _userRepo.GetQueryable()
                .CountAsync(x => x.Role == "Patient" && x.IsActive);

            var totalStaff = await _staffRepo.GetQueryable()
                .CountAsync(x => x.IsAvailable);

            var totalAppointments = await _appointmentRepo.GetQueryable()
                .CountAsync();

            var todayAppointments = await _appointmentRepo.GetQueryable()
                .CountAsync(x => x.AppointmentDate.Date == today);

            var todayApprovedAppointments = await _appointmentRepo.GetQueryable()
                .CountAsync(x =>
                    x.AppointmentDate.Date == today &&
                    x.Status == PatientStatus.Approved);

            var todayPendingAppointments = await _appointmentRepo.GetQueryable()
    .CountAsync(x =>
        x.AppointmentDate.Date == today &&
        x.Status == PatientStatus.Pending);

            var todayCompletedAppointments = await _appointmentRepo.GetQueryable()
    .CountAsync(x =>
        x.AppointmentDate.Date == today &&
        x.Status == PatientStatus.Completed);

            var todayCancelledAppointments = await _appointmentRepo.GetQueryable()
    .CountAsync(x =>
        x.AppointmentDate.Date == today &&
        x.Status == PatientStatus.Cancelled);


            var pendingAppointments = await _appointmentRepo.GetQueryable()
                .CountAsync(x => x.Status == PatientStatus.Pending);

            var approvedAppointments = await _appointmentRepo.GetQueryable()
                .CountAsync(x => x.Status == PatientStatus.Approved);

            var completedAppointments = await _appointmentRepo.GetQueryable()
                .CountAsync(x => x.Status == PatientStatus.Completed);

            var cancelledAppointments = await _appointmentRepo.GetQueryable()
                .CountAsync(x => x.Status == PatientStatus.Cancelled);

            // ================= CONVERSION RATE =================

            double conversionRate = 0;

            if (totalAppointments > 0)
            {
                conversionRate =
                    Math.Round(((double)approvedAppointments / totalAppointments) * 100, 2);
            }

            // ================= MOST BOOKED DAY =================

            var mostBookedDay = await _appointmentRepo.GetQueryable()
                .GroupBy(x => x.AppointmentDate.DayOfWeek)
                .Select(g => new
                {
                    Day = g.Key.ToString(),
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Count)
                .FirstOrDefaultAsync();

            // ================= MOST BOOKED SLOT =================

            var mostBookedSlot = await _appointmentRepo.GetQueryable()
                .GroupBy(x => x.SlotTime)
                .Select(g => new
                {
                    Slot = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Count)
                .FirstOrDefaultAsync();

            // ================= LAST 7 DAYS TREND =================

            var last7Days = today.AddDays(-6);

            var trendData = await _appointmentRepo.GetQueryable()
        .Where(x => x.AppointmentDate.Date >= last7Days)
        .GroupBy(x => x.AppointmentDate.Date)
        .Select(g => new
        {
            Day = g.Key,
            Count = g.Count()
        })
        .OrderBy(x => x.Day)
        .ToListAsync();

            var trend = trendData.Select(x => new AppointmentTrendDto
            {
                Day = x.Day.ToString("dd MMM"),
                Count = x.Count
            }).ToList();

            // ================= MOST BOOKED STAFF =================

            var mostBookedStaff = await _appointmentRepo.GetQueryable()
                .Where(x => x.StaffId != null)
                .GroupBy(x => x.Staff.User.Name)
                .Select(g => new
                {
                    Name = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Count)
                .FirstOrDefaultAsync();

            // ================= LEAST ACTIVE STAFF =================

            var leastActiveStaff = await _appointmentRepo.GetQueryable()
                .Where(x => x.StaffId != null)
                .GroupBy(x => x.Staff.User.Name)
                .Select(g => new
                {
                    Name = g.Key,
                    Count = g.Count()
                })
                .OrderBy(x => x.Count)
                .FirstOrDefaultAsync();

            // ================= BUSY STAFF TODAY =================

            var busyStaffToday = await _appointmentRepo.GetQueryable()
                .Where(x =>
                    x.AppointmentDate.Date == today &&
                    x.Status == PatientStatus.Approved &&
                    x.StaffId != null)
                .Select(x => x.StaffId)
                .Distinct()
                .CountAsync();

            // ================= STAFF WITHOUT AVAILABILITY =================

            var staffWithoutAvailability = await _staffRepo.GetQueryable()
                .CountAsync(x =>
                    !_availabilityRepo.GetQueryable()
                        .Any(a => a.StaffId == x.Id && a.IsActive));

            // ================= MOST BOOKED SERVICE =================

            var mostBookedService = await _appointmentRepo.GetQueryable()
                .GroupBy(x => x.Service.Name)
                .Select(g => new
                {
                    Name = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Count)
                .FirstOrDefaultAsync();

            // ================= LEAST BOOKED SERVICE =================

            var leastBookedService = await _appointmentRepo.GetQueryable()
                .GroupBy(x => x.Service.Name)
                .Select(g => new
                {
                    Name = g.Key,
                    Count = g.Count()
                })
                .OrderBy(x => x.Count)
                .FirstOrDefaultAsync();

            // ================= SERVICE WISE BOOKINGS =================

            var serviceWiseBookings = await _appointmentRepo.GetQueryable()
                .GroupBy(x => x.Service.Name)
                .Select(g => new ServiceBookingDto
                {
                    ServiceName = g.Key,
                    TotalBookings = g.Count()
                })
                .OrderByDescending(x => x.TotalBookings)
                .ToListAsync();

            // ================= RECENT APPOINTMENTS =================

            var recentAppointments = await _appointmentRepo.GetQueryable()
                .OrderByDescending(x => x.Id)
                .Take(10)
                .Select(x => new RecentAppointmentDto
                {
                    Id = x.Id,
                    PatientName = x.User.Name,
                    ServiceName = x.Service.Name,
                    StaffName = x.Staff != null ? x.Staff.User.Name : null,
                    AppointmentDate = x.AppointmentDate,
                    SlotTime = x.SlotTime,
                    Status = (int)x.Status
                })
                .ToListAsync();

            // ================= FINAL RESPONSE =================

            var result = new AdminDashboardDto
            {
                Overview = new DashboardOverviewDto
                {
                    TotalPatients = totalPatients,
                    TotalStaff = totalStaff,
                    TotalAppointments = totalAppointments,
                    TodayAppointments = todayAppointments,
                    TodayApprovedAppointments = todayApprovedAppointments,
                    TodayPendingAppointments = todayPendingAppointments,

                    TodayCompletedAppointments = todayCompletedAppointments,

                    TodayCancelledAppointments = todayCancelledAppointments,
                    PendingAppointments = pendingAppointments,
                    ApprovedAppointments = approvedAppointments,
                    CompletedAppointments = completedAppointments,
                    CancelledAppointments = cancelledAppointments
                },

                AppointmentAnalytics = new AppointmentAnalyticsDto
                {
                    ConversionRate = conversionRate,
                    MostBookedDay = mostBookedDay?.Day,
                    MostBookedSlot = mostBookedSlot?.Slot,
                    Last7DaysTrend = trend
                },

                StaffAnalytics = new StaffAnalyticsDto
                {
                    MostBookedStaff = mostBookedStaff?.Name,
                    LeastActiveStaff = leastActiveStaff?.Name,
                    BusyStaffToday = busyStaffToday,
                    StaffWithoutAvailability = staffWithoutAvailability
                },

                ServiceAnalytics = new ServiceAnalyticsDto
                {
                    MostBookedService = mostBookedService?.Name,
                    LeastBookedService = leastBookedService?.Name,
                    ServiceWiseBookings = serviceWiseBookings
                },

                RecentAppointments = recentAppointments
            };

            return ApiResponse<AdminDashboardDto>.SuccessResponse(
                result,
                "Dashboard fetched successfully"
            );
        }
    }
}

using PatientBookingSystem.Application.DTOs;
using PatientBookingSystem.Application.DTOs.Common;
using PatientBookingSystem.Application.Interfaces;
using PatientBookingSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace PatientBookingSystem.Application.Services
{
    public class StaffAvailabilityService : IStaffAvailabilityService
    {
        private readonly IStaffAvailabilityRepository _repo;

        public StaffAvailabilityService(IStaffAvailabilityRepository repo)
        {
            _repo = repo;
        }

        public async Task<ApiResponse<string>> CreateAsync(CreateStaffAvailabilityDto dto)
        {
            // ❌ Validation
            if (dto.StartTime >= dto.EndTime)
                return ApiResponse<string>.FailResponse("Start time must be less than end time");

            // ❌ Overlap check
            var exists = await _repo.GetQueryable().AnyAsync(x =>
                x.StaffId == dto.StaffId &&
                x.Day == dto.Day &&
                x.IsActive &&
                (
                    dto.StartTime < x.EndTime &&
                    dto.EndTime > x.StartTime
                )
            );

            if (exists)
                return ApiResponse<string>.FailResponse("Time slot overlaps");

            var entity = new StaffAvailability
            {
                StaffId = dto.StaffId,
                Day = dto.Day,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                CreatedAt = DateTime.UtcNow
            };

            await _repo.AddAsync(entity);
            await _repo.SaveChangesAsync();

            return ApiResponse<string>.SuccessResponse("Availability created");
        }

        public async Task<ApiResponse<List<StaffAvailabilityDto>>> GetByStaffId(int staffId)
        {
            var data = await _repo.GetQueryable()
                .Where(x => x.StaffId == staffId && x.IsActive)
                .Select(x => new StaffAvailabilityDto
                {
                    Id = x.Id,
                    StaffId = x.StaffId,
                    Day = x.Day,
                    StartTime = x.StartTime,
                    EndTime = x.EndTime
                })
                .ToListAsync();

            return ApiResponse<List<StaffAvailabilityDto>>.SuccessResponse(data, "Fetched");
        }
    }
}

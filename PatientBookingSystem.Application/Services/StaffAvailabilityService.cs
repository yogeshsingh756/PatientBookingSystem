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

        public async Task<ApiResponse<StaffAvailabilityDto>> GetByIdAsync(int id)
        {
            var entity = await _repo.GetQueryable()
                .FirstOrDefaultAsync(x => x.Id == id && x.IsActive);

            if (entity == null)
                return ApiResponse<StaffAvailabilityDto>.FailResponse("Availability not found");

            var result = new StaffAvailabilityDto
            {
                Id = entity.Id,
                StaffId = entity.StaffId,
                Day = entity.Day,
                StartTime = entity.StartTime,
                EndTime = entity.EndTime
            };

            return ApiResponse<StaffAvailabilityDto>.SuccessResponse(result, "Fetched successfully");
        }

        public async Task<ApiResponse<string>> UpdateAsync(UpdateStaffAvailabilityDto dto)
        {
            var entity = await _repo.GetQueryable()
                .FirstOrDefaultAsync(x => x.Id == dto.Id && x.IsActive);

            if (entity == null)
                return ApiResponse<string>.FailResponse("Availability not found");

            // ❌ Validate time
            if (dto.StartTime >= dto.EndTime)
                return ApiResponse<string>.FailResponse("Start time must be less than end time");

            // ❌ Overlap check (exclude current record)
            var exists = await _repo.GetQueryable().AnyAsync(x =>
                x.Id != dto.Id &&
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

            // ✅ Update
            entity.StaffId = dto.StaffId;
            entity.Day = dto.Day;
            entity.StartTime = dto.StartTime;
            entity.EndTime = dto.EndTime;

            await _repo.SaveChangesAsync();

            return ApiResponse<string>.SuccessResponse("Availability updated");
        }

        public async Task<ApiResponse<string>> DeleteAsync(int id)
        {
            var entity = await _repo.GetQueryable()
                .FirstOrDefaultAsync(x => x.Id == id && x.IsActive);

            if (entity == null)
                return ApiResponse<string>.FailResponse("Availability not found");

            entity.IsActive = false;

            await _repo.SaveChangesAsync();

            return ApiResponse<string>.SuccessResponse("Availability deleted");
        }
    }
}

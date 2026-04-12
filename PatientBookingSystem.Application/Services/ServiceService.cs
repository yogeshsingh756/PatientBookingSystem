using Microsoft.AspNetCore.Http;
using PatientBookingSystem.Application.DTOs;
using PatientBookingSystem.Application.DTOs.Common;
using PatientBookingSystem.Application.Interfaces;
using PatientBookingSystem.Domain.Entities;
using System.Buffers.Text;

namespace PatientBookingSystem.Application.Services
{
    public class ServiceService : IServiceService
    {
        private readonly IServiceRepository _repo;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ServiceService(IServiceRepository repo, IHttpContextAccessor httpContextAccessor)
        {
            _repo = repo;
            _httpContextAccessor = httpContextAccessor;
        }

        // ✅ CREATE
        public async Task<ApiResponse<string>> CreateAsync(CreateServiceDto dto)
        {
            var imagePath = await SaveImage(dto.Image);

            var service = new Service
            {
                Name = dto.Name,
                Category = dto.Category,
                Description = dto.Description,
                ImageUrl = imagePath,
                Icon = dto.Icon,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _repo.AddAsync(service);
            await _repo.SaveChangesAsync(); // ✅ IMPORTANT

            return ApiResponse<string>.SuccessResponse("Service created successfully");
        }

        // ✅ UPDATE
        public async Task<ApiResponse<string>> UpdateAsync(int id, CreateServiceDto dto)
        {
            var service = await _repo.GetByIdAsync(id);

            if (service == null)
                return ApiResponse<string>.FailResponse("Service not found");

            if (dto.Image != null)
                service.ImageUrl = await SaveImage(dto.Image);

            service.Name = dto.Name;
            service.Category = dto.Category;
            service.Description = dto.Description;
            service.Icon = dto.Icon;

            await _repo.UpdateAsync(service);     // ✅ IMPORTANT
            await _repo.SaveChangesAsync();       // ✅ IMPORTANT

            return ApiResponse<string>.SuccessResponse("Service updated successfully");
        }

        // ✅ DELETE (SOFT DELETE)
        public async Task<ApiResponse<string>> DeleteAsync(int id)
        {
            var service = await _repo.GetByIdAsync(id);

            if (service == null)
                return ApiResponse<string>.FailResponse("Service not found");

            service.IsActive = false;

            await _repo.UpdateAsync(service);     // ✅ IMPORTANT
            await _repo.SaveChangesAsync();       // ✅ IMPORTANT

            return ApiResponse<string>.SuccessResponse("Service deleted successfully");
        }

        // ✅ GET ALL
        public async Task<ApiResponse<List<ServiceDto>>> GetAllAsync()
        {
            var services = await _repo.GetAllAsync();
            var request = _httpContextAccessor.HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}";

            var data = services
                .Where(x => x.IsActive)
                .Select(x => new ServiceDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Category = x.Category,
                    Description = x.Description,
                    ImageUrl = string.IsNullOrEmpty(x.ImageUrl)
                ? null
                : baseUrl + x.ImageUrl,
                    Icon = x.Icon
                })
                .ToList();

            return ApiResponse<List<ServiceDto>>.SuccessResponse(data, "Services fetched successfully");
        }

        // ✅ GET BY ID
        public async Task<ApiResponse<ServiceDto>> GetByIdAsync(int id)
        {
            var service = await _repo.GetByIdAsync(id);
            var request = _httpContextAccessor.HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}";
            if (service == null)
                return ApiResponse<ServiceDto>.FailResponse("Service not found");

            var data = new ServiceDto
            {
                Id = service.Id,
                Name = service.Name,
                Category = service.Category,
                Description = service.Description,
                ImageUrl = string.IsNullOrEmpty(service.ImageUrl)
                ? null
                : baseUrl + service.ImageUrl,
                Icon = service.Icon
            };

            return ApiResponse<ServiceDto>.SuccessResponse(data);
        }

        // ✅ IMAGE SAVE METHOD
        private async Task<string?> SaveImage(IFormFile? file)
        {
            if (file == null) return null;

            var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/services");

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(folder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return "/uploads/services/" + fileName;
        }
    }
}
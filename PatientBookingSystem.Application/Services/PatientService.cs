using Microsoft.AspNetCore.Http;
using PatientBookingSystem.Application.DTOs;
using PatientBookingSystem.Application.DTOs.Common;
using PatientBookingSystem.Application.Interfaces;
using PatientBookingSystem.Domain.Entities;
using PatientBookingSystem.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace PatientBookingSystem.Application.Services
{
    public class PatientService : IPatientService
    {
        private readonly IPatientRepository _repo;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IPatientStatusHistoryRepository _historyRepo;

        public PatientService(IPatientRepository repo, IHttpContextAccessor httpContext, IPatientStatusHistoryRepository historyRepo)
        {
            _repo = repo;
            _httpContext = httpContext;
            _historyRepo = historyRepo;
        }

        // ✅ CREATE
        public async Task<ApiResponse<string>> CreateAsync(CreatePatientDto dto)
        {
            try
            {
                var imagePath = await SaveImage(dto.DiseaseImage);

                var patient = new PatientAppointment
                {
                    UserId = dto.UserId,
                    ServiceId = dto.ServiceId,
                    StaffId = dto.StaffId.GetValueOrDefault() == 0 ? null : dto.StaffId,
                    AppointmentDate = dto.AppointmentDate,
                    SlotTime = dto.SlotTime,
                    NoOfDays = dto.NoOfDays,
                    DiseaseName = dto.DiseaseName,
                    DischargeDate = dto.DischargeDate,
                    DoctorPrescription = dto.DoctorPrescription,
                    DiseaseImageUrl = imagePath,
                    Status = PatientStatus.Pending,
                    CreatedAt = DateTime.UtcNow
                };

                await _repo.AddAsync(patient);

                // Note: Ensure patient.Id is populated. 
                // If your DB generates IDs on Save, move SaveChangesAsync before adding history.
                await _historyRepo.AddAsync(new PatientAppointmentStatusHistory
                {
                    PatientId = patient.Id,
                    Status = PatientStatus.Pending,
                    Remarks = "Appointment created"
                });

                await _repo.SaveChangesAsync();

                return ApiResponse<string>.SuccessResponse("Patient Appointment created successfully");
            }
            catch (Exception ex)
            {
                return null;
                // Log the exception here (e.g., _logger.LogError(ex.Message))
            }
        }

        // ✅ GET ALL
        public async Task<ApiResponse<List<PatientDto>>> GetAllAsync()
        {
            var data = await _repo.GetAllAsync();

            var request = _httpContext.HttpContext?.Request;
            var baseUrl = request != null ? $"{request.Scheme}://{request.Host}" : "";

            var result = data.Select(x => new PatientDto
            {
                Id = x.Id,
                UserId = x.UserId,
                ServiceId = x.ServiceId ??0,
                StaffId = x.StaffId,
                AppointmentDate = x.AppointmentDate,
                SlotTime = x.SlotTime,
                NoOfDays = x.NoOfDays,
                DiseaseName = x.DiseaseName,
                DischargeDate = x.DischargeDate,
                DoctorPrescription = x.DoctorPrescription,
                DiseaseImageUrl = x.DiseaseImageUrl != null ? baseUrl + x.DiseaseImageUrl : null,
                Status = x.Status.ToString()
            }).ToList();

            return ApiResponse<List<PatientDto>>.SuccessResponse(result, "Patients fetched successfully");
        }

        // Get All Appoinments For Specific Users

        public async Task<ApiResponse<List<PatientUserAppointmentDto>>> GetAppoinmentsByUserId(int userId)
        {
            var request = _httpContext.HttpContext?.Request;
            var baseUrl = request != null ? $"{request.Scheme}://{request.Host}" : "";

            var data = await _repo.GetQueryable()
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.Id)
                .Select(x => new PatientUserAppointmentDto
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    UserName = x.User.Name ?? string.Empty,
                    Email = x.User.Email ?? string.Empty,
                    PhoneNumber = x.User.PhoneNumber ?? string.Empty,
                    Address = x.User.Address ?? string.Empty,
                    Landmark = x.User.Landmark ?? string.Empty,
                    HouseNumber = x.User.HouseNumber ?? string.Empty,
                    PinCode = x.User.PinCode ?? string.Empty,
                    DischargeDate = x.DischargeDate ?? null,
                    DoctorPrescription =  x.DoctorPrescription ?? string.Empty,
                    NoOfDays = x.NoOfDays ?? 0,
                    Remarks = _historyRepo.GetQueryable()
                        .Where(h => h.PatientId == x.Id)
                        .OrderByDescending(h => h.Id)
                        .Select(h => h.Remarks)
                        .FirstOrDefault() ?? string.Empty,
                    ServiceId = x.ServiceId ?? 0,
                    StaffId = x.StaffId ?? 0,
                    StaffName = string.Empty, // Assuming you have a way to get staff name if needed
                    AppointmentDate = x.AppointmentDate,
                    SlotTime = x.SlotTime,
                    DiseaseName = x.DiseaseName,
                    ServiceName = x.Service.Name ?? string.Empty,
                    DiseaseImageUrl = x.DiseaseImageUrl != null
                        ? baseUrl + x.DiseaseImageUrl
                        : null,
                    Status = x.Status.ToString() ?? string.Empty,
                })
                .ToListAsync();

            if (data == null || data.Count == 0)
                return ApiResponse<List<PatientUserAppointmentDto>>
                    .SuccessResponse(new List<PatientUserAppointmentDto>(), "No appointments found");

            return ApiResponse<List<PatientUserAppointmentDto>>
                .SuccessResponse(data, "Appointments fetched successfully");
        }

        // ✅ GET BY ID
        public async Task<ApiResponse<PatientDto>> GetByIdAsync(int id)
        {
            var x = await _repo.GetByIdAsync(id);

            if (x == null)
                return ApiResponse<PatientDto>.FailResponse("Patient not found");

            var request = _httpContext.HttpContext?.Request;
            var baseUrl = request != null ? $"{request.Scheme}://{request.Host}" : "";

            var data = new PatientDto
            {
                Id = x.Id,
                UserId = x.UserId,
                ServiceId = x.ServiceId ?? 0,
                StaffId = x.StaffId,
                AppointmentDate = x.AppointmentDate,
                SlotTime = x.SlotTime,
                NoOfDays = x.NoOfDays,
                DiseaseName = x.DiseaseName,
                DischargeDate = x.DischargeDate,
                DoctorPrescription = x.DoctorPrescription,
                DiseaseImageUrl = x.DiseaseImageUrl != null ? baseUrl + x.DiseaseImageUrl : null,
                Status = x.Status.ToString()
            };

            return ApiResponse<PatientDto>.SuccessResponse(data, "Patient Appoinment fetched successfully");
        }

        // ✅ UPDATE
        public async Task<ApiResponse<string>> UpdateAsync(int id, CreatePatientDto dto)
        {
            var patient = await _repo.GetByIdAsync(id);

            if (patient == null)
                return ApiResponse<string>.FailResponse("Patient not found");

            if (dto.DiseaseImage != null)
                patient.DiseaseImageUrl = await SaveImage(dto.DiseaseImage);

            patient.UserId = dto.UserId;
            patient.ServiceId = dto.ServiceId;
            patient.StaffId = dto.StaffId.GetValueOrDefault() == 0 ? null : dto.StaffId;
            patient.AppointmentDate = dto.AppointmentDate;
            patient.SlotTime = dto.SlotTime;
            patient.NoOfDays = dto.NoOfDays;
            patient.DiseaseName = dto.DiseaseName;
            patient.DischargeDate = dto.DischargeDate;
            patient.DoctorPrescription = dto.DoctorPrescription;

            await _repo.UpdateAsync(patient);
            await _repo.SaveChangesAsync();

            return ApiResponse<string>.SuccessResponse("Patient Appoinment updated successfully");
        }

        // ✅ DELETE (SOFT DELETE STYLE VIA STATUS)
        public async Task<ApiResponse<string>> DeleteAsync(int id)
        {
            var patient = await _repo.GetByIdAsync(id);

            if (patient == null)
                return ApiResponse<string>.FailResponse("Appointment not found");

            if (patient.Status == PatientStatus.Completed)
                return ApiResponse<string>.FailResponse("Completed appointment cannot be cancelled");

            patient.Status = PatientStatus.Cancelled;

            await _repo.UpdateAsync(patient);
            await _repo.SaveChangesAsync();

            return ApiResponse<string>.SuccessResponse("Appointment cancelled successfully");
        }

        public async Task<ApiResponse<string>> ChangeStatusAsync(ChangePatientStatusDto dto)
        {
            var patient = await _repo.GetByIdAsync(dto.Id);

            if (patient == null)
                return ApiResponse<string>.FailResponse("Patient not found");

            // 🔒 Cannot modify completed
            if (patient.Status == PatientStatus.Completed)
                return ApiResponse<string>.FailResponse("Completed appointment cannot be changed");

            // 🔁 Prevent same status update
            if (patient.Status == dto.Status)
                return ApiResponse<string>.FailResponse("Status is already the same");

            // ✅ Update status
            patient.Status = dto.Status;

            await _repo.UpdateAsync(patient);

            // ✅ Add history
            await _historyRepo.AddAsync(new PatientAppointmentStatusHistory
            {
                PatientId = patient.Id,
                Status = dto.Status,
                Remarks = dto.Remarks
            });

            // ✅ Single save
            await _repo.SaveChangesAsync();

            return ApiResponse<string>.SuccessResponse($"Status updated to {dto.Status}");
        }


        public async Task<ApiResponse<object>> GetAllWithPaginationAsync(PaginationRequestDto dto)
        {
            var query = _repo.GetQueryable();

            // 🔍 SEARCH
            if (!string.IsNullOrEmpty(dto.Search))
            {
                var search = dto.Search.ToLower();

                query = query.Where(x =>
                    x.User.Name.ToLower().Contains(search) ||
                    x.User.PhoneNumber.Contains(search) ||
                    x.DiseaseName.ToLower().Contains(search) ||
                    x.Service.Name.ToLower().Contains(search)
                );
            }

            // 📊 TOTAL COUNT
            var totalRecords = await query.CountAsync();

            // 📉 PAGINATION
            var data = await query
                .OrderByDescending(x => x.Id)
                .Skip((dto.PageNumber - 1) * dto.PageSize)
                .Take(dto.PageSize)
                .Select(x => new PatientAppoinmentListDto
                {
                    Id = x.Id,

                    UserName = x.User.Name,
                    PhoneNumber = x.User.PhoneNumber,

                    Address = x.User.Address,
                    HouseNumber = x.User.HouseNumber,
                    PinCode = x.User.PinCode,

                    ServiceName = x.Service.Name,
                    Category = x.Service.Category,

                    AppointmentDate = x.AppointmentDate,
                    SlotTime = x.SlotTime,

                    DiseaseName = x.DiseaseName,
                    Status = (int)x.Status,
                    NoOfDays = x.NoOfDays
                })
                .ToListAsync();

            return ApiResponse<object>.SuccessResponse(new
            {
                TotalRecords = totalRecords,
                PageNumber = dto.PageNumber,
                PageSize = dto.PageSize,
                Data = data
            }, "Appointments fetched successfully");
        }
         //✅ IMAGE SAVE
        private async Task<string?> SaveImage(IFormFile? file)
        {
            if (file == null) return null;

            var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/patient");

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var path = Path.Combine(folder, fileName);

            using var stream = new FileStream(path, FileMode.Create);
            await file.CopyToAsync(stream);

            return "/uploads/patient/" + fileName;
        }
    }
}

using Microsoft.AspNetCore.Http;
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
            var imagePath = await SaveImage(dto.DiseaseImage);

            var patient = new Patient
            {
                UserId = dto.UserId,
                ServiceId = dto.ServiceId,
                StaffId = dto.StaffId,
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
            
            await _historyRepo.AddAsync(new PatientStatusHistory
            {
                PatientId = patient.Id,
                Status = PatientStatus.Pending,
                Remarks = "Appointment created"
            });
            await _repo.SaveChangesAsync();

            return ApiResponse<string>.SuccessResponse("Patient created successfully");
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
                ServiceId = x.ServiceId,
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
                ServiceId = x.ServiceId,
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

            return ApiResponse<PatientDto>.SuccessResponse(data, "Patient fetched successfully");
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
            patient.StaffId = dto.StaffId;
            patient.AppointmentDate = dto.AppointmentDate;
            patient.SlotTime = dto.SlotTime;
            patient.NoOfDays = dto.NoOfDays;
            patient.DiseaseName = dto.DiseaseName;
            patient.DischargeDate = dto.DischargeDate;
            patient.DoctorPrescription = dto.DoctorPrescription;

            await _repo.UpdateAsync(patient);
            await _repo.SaveChangesAsync();

            return ApiResponse<string>.SuccessResponse("Patient updated successfully");
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
            await _historyRepo.AddAsync(new PatientStatusHistory
            {
                PatientId = patient.Id,
                Status = dto.Status,
                Remarks = dto.Remarks
            });

            // ✅ Single save
            await _repo.SaveChangesAsync();

            return ApiResponse<string>.SuccessResponse($"Status updated to {dto.Status}");
        }

        // ✅ IMAGE SAVE
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

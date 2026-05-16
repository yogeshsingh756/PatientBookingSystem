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
        private readonly IStaffRepository _staffRepo;
        private readonly INotificationService _notificationService;

        public PatientService(IPatientRepository repo, IHttpContextAccessor httpContext, IPatientStatusHistoryRepository historyRepo, IStaffRepository staffRepo, INotificationService notification)
        {
            _repo = repo;
            _httpContext = httpContext;
            _historyRepo = historyRepo;
            _staffRepo = staffRepo;
            _notificationService = notification;
        }

        // ✅ CREATE
        public async Task<ApiResponse<string>> CreateAsync(CreatePatientDto dto)
        {
            try
            {
                var imagePath = await SaveImage(dto.DiseaseImage);
                var prescriptionImagePath = await SaveImage(dto.DoctorPrescriptionImage);

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
                    CreatedAt = DateTime.UtcNow,
                    DoctorPrescriptionImageUrl = prescriptionImagePath,
                    Latitude = dto.Latitude,
                    Longitude = dto.Longitude,
                    AppointmentAddress = dto.AppointmentAddress
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
                ServiceId = x.ServiceId ?? 0,
                StaffId = x.StaffId,
                AppointmentDate = x.AppointmentDate,
                SlotTime = x.SlotTime,
                NoOfDays = x.NoOfDays,
                DiseaseName = x.DiseaseName,
                DischargeDate = x.DischargeDate,
                DoctorPrescription = x.DoctorPrescription,
                DoctorPrescriptionImageUrl = x.DoctorPrescriptionImageUrl != null ? baseUrl + x.DoctorPrescriptionImageUrl : null,
                DiseaseImageUrl = x.DiseaseImageUrl != null ? baseUrl + x.DiseaseImageUrl : null,
                Status = x.Status.ToString(),
                Latitude = x.Latitude,
                Longitude = x.Longitude,
                AppointmentAddress = x.AppointmentAddress
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
                    DoctorPrescription = x.DoctorPrescription ?? string.Empty,
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
                    DoctorPrescriptionImageUrl = x.DoctorPrescriptionImageUrl != null
                        ? baseUrl + x.DoctorPrescriptionImageUrl
                        : null,
                    DiseaseImageUrl = x.DiseaseImageUrl != null
                        ? baseUrl + x.DiseaseImageUrl
                        : null,
                    Status = x.Status.ToString() ?? string.Empty,
                    Latitude = x.Latitude,
                    Longitude = x.Longitude,
                    AppointmentAddress = x.AppointmentAddress ?? string.Empty
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
                DoctorPrescriptionImageUrl = x.DoctorPrescriptionImageUrl != null ? baseUrl + x.DoctorPrescriptionImageUrl : null,
                DiseaseImageUrl = x.DiseaseImageUrl != null ? baseUrl + x.DiseaseImageUrl : null,
                Status = x.Status.ToString(),
                Latitude = x.Latitude,
                Longitude = x.Longitude,
                AppointmentAddress = x.AppointmentAddress
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
            if (dto.DoctorPrescriptionImage != null)
                patient.DoctorPrescriptionImageUrl = await SaveImage(dto.DoctorPrescriptionImage);

            patient.UserId = dto.UserId;
            patient.ServiceId = dto.ServiceId;
            patient.StaffId = dto.StaffId.GetValueOrDefault() == 0 ? null : dto.StaffId;
            patient.AppointmentDate = dto.AppointmentDate;
            patient.SlotTime = dto.SlotTime;
            patient.NoOfDays = dto.NoOfDays;
            patient.DiseaseName = dto.DiseaseName;
            patient.DischargeDate = dto.DischargeDate;
            patient.DoctorPrescription = dto.DoctorPrescription;
            patient.Latitude = dto.Latitude;
            patient.Longitude = dto.Longitude;
            patient.AppointmentAddress = dto.AppointmentAddress;

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
            var patient = await _repo.GetQueryable()
               .Include(x => x.User)
               .Include(x => x.Service)
               .Include(x => x.Staff)
                .ThenInclude(s => s.User)
               .FirstOrDefaultAsync(x => x.Id == dto.Id);

            if (patient == null)
                return ApiResponse<string>.FailResponse("Patient not found");

            // 🔒 Cannot modify completed
            if (patient.Status == PatientStatus.Completed)
                return ApiResponse<string>.FailResponse("Completed appointment cannot be changed");

            // 🔁 Prevent same status update
            if (patient.Status == dto.Status)
                return ApiResponse<string>.FailResponse("Status is already the same");
            // 🔥 APPROVAL LOGIC
            if (dto.Status == PatientStatus.Approved)
            {
                // ✅ Staff required during approval
                var finalStaffId = dto.StaffId ?? patient.StaffId;

                if (finalStaffId == null)
                    return ApiResponse<string>.FailResponse("Staff is required for approval");

                // ✅ Check staff exists
                var staffExists = await _staffRepo.GetQueryable()
                    .AnyAsync(x => x.Id == finalStaffId && x.IsAvailable);

                if (!staffExists)
                    return ApiResponse<string>.FailResponse("Selected staff not found");

                // ✅ Recheck availability
                var alreadyBooked = await _repo.GetQueryable()
                    .AnyAsync(x =>
                        x.Id != patient.Id &&
                        x.StaffId == finalStaffId &&
                        x.AppointmentDate.Date == patient.AppointmentDate.Date &&
                        x.SlotTime == patient.SlotTime &&
                        x.Status == PatientStatus.Approved
                    );

                if (alreadyBooked)
                    return ApiResponse<string>.FailResponse("Staff is no longer available");

                // ✅ Assign staff
                patient.StaffId = finalStaffId;
            }

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

            if(dto.Status == PatientStatus.Approved)
            {

              patient = await _repo.GetQueryable()
              .Include(x => x.User)
              .Include(x => x.Service)
              .Include(x => x.Staff)
               .ThenInclude(s => s.User)
              .FirstOrDefaultAsync(x => x.Id == patient.Id);

            }
            // ================= NOTIFICATIONS =================

            await SendStatusNotifications(patient, dto);

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
                    NoOfDays = x.NoOfDays,
                    Latitude = x.Latitude,
                    Longitude = x.Longitude,
                    AppointmentAddress = x.AppointmentAddress,
                    StaffId = x.StaffId ?? 0,
                    StaffName = x.Staff != null && x.Staff.User != null ? x.Staff.User.Name : string.Empty
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
        private async Task SendStatusNotifications(PatientAppointment patient, ChangePatientStatusDto dto)
        {
            try
            {
                switch (dto.Status)
                {
                    case PatientStatus.Approved:

                        await SendApprovalNotifications(patient);

                        break;

                    case PatientStatus.Completed:

                        await SendCompletedNotification(patient);

                        break;

                    case PatientStatus.Cancelled:

                        await SendCancelledNotification(patient, dto.Remarks);

                        break;
                }
            }
            catch
            {
                // Ignore notification failures
            }
        }
        private async Task SendApprovalNotifications(PatientAppointment patient)
        {
            string googleMapLink = "";

            if (patient.Latitude != null && patient.Longitude != null)
            {
                googleMapLink =
                    $"https://www.google.com/maps?q={patient.Latitude},{patient.Longitude}";
            }

            // ================= PATIENT =================

            var patientMessage = $@"
            <h3>Appointment Approved</h3>

            <p>Hello {patient.User.Name},</p>

            <p>Your appointment has been approved.</p>

            <p>
              <b>Service:</b> {patient.Service?.Name}<br/>
              <b>Date:</b> {patient.AppointmentDate:dd MMM yyyy}<br/>
              <b>Slot:</b> {patient.SlotTime}<br/>
              <b>Staff:</b> {patient.Staff?.User?.Name}
            </p>

            <p>Thank you.</p>
            ";

            await SafeSendEmail(
                patient.User.Email,
                "Appointment Approved",
                patientMessage);

            await SafeSendSms(
                patient.User.PhoneNumber,
                $"Your appointment on {patient.AppointmentDate:dd MMM} at {patient.SlotTime} has been approved.");

            // ================= STAFF =================

            if (patient.Staff?.User != null)
            {
                var staffMessage = $@"
                 <h3>New Appointment Assigned</h3>

                 <p>Hello {patient.Staff.User.Name},</p>

                 <p>You have been assigned a patient visit.</p>

                 <p>
                  <b>Patient:</b> {patient.User.Name}<br/>
                  <b>Phone:</b> {patient.User.PhoneNumber}<br/>
                  <b>Date:</b> {patient.AppointmentDate:dd MMM yyyy}<br/>
                  <b>Slot:</b> {patient.SlotTime}<br/>
                  <b>Address:</b> {patient.AppointmentAddress}
                 </p>

                 <p>
                   <a href='{googleMapLink}'>
                    Open Google Map
                   </a>
                 </p>
                ";

                await SafeSendEmail(
                    patient.Staff.User.Email,
                    "New Appointment Assigned",
                    staffMessage);

                await SafeSendSms(
                    patient.Staff.User.PhoneNumber,
                    $"New appointment assigned on {patient.AppointmentDate:dd MMM} at {patient.SlotTime}. Map: {googleMapLink}");
            }
        }
        private async Task SendCompletedNotification(PatientAppointment patient)
        {
            var message = $@"
             <h3>Appointment Completed</h3>

             <p>Hello {patient.User.Name},</p>

             <p>Your appointment has been completed successfully.</p>

             <p>Thank you for choosing us.</p>
            ";

            await SafeSendEmail(
                patient.User.Email,
                "Appointment Completed",
                message);

            await SafeSendSms(
                patient.User.PhoneNumber,
                "Your appointment has been completed successfully.");
        }
        private async Task SendCancelledNotification(PatientAppointment patient, string? remarks)
        {
            var message = $@"
             <h3>Appointment Cancelled</h3>

             <p>Hello {patient.User.Name},</p>

             <p>Your appointment has been cancelled.</p>

             <p>
             <b>Reason:</b> {remarks}
             </p>
            ";

            await SafeSendEmail(
                patient.User.Email,
                "Appointment Cancelled",
                message);

            await SafeSendSms(
                patient.User.PhoneNumber,
                $"Your appointment has been cancelled. Reason: {remarks}");
        }
        private async Task SafeSendEmail(string? email, string subject, string message)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(email))
                {
                    await _notificationService
                        .SendEmailAsync(email, subject, message);
                }
            }
            catch
            {
                // Ignore email failure
            }
        }
        private async Task SafeSendSms( string? phone, string message)
        {
            try
            {
                //if (!string.IsNullOrWhiteSpace(phone))
                //{
                //    await _notificationService
                //        .SendSmsAsync(phone, message);
                //}
            }
            catch
            {
                // Ignore SMS failure
            }
        }
    }
}

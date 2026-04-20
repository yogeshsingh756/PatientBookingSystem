using PatientBookingSystem.Application.DTOs;
using PatientBookingSystem.Application.DTOs.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace PatientBookingSystem.Application.Interfaces
{
    public interface IPatientService
    {
        //  CREATE PATIENT (Appointment + History)
        Task<ApiResponse<string>> CreateAsync(CreatePatientDto dto);

        //  GET ALL PATIENTS
        Task<ApiResponse<List<PatientDto>>> GetAllAsync();

        // Get All Appoinments for a specific patient
        Task<ApiResponse<List<PatientUserAppointmentDto>>> GetAppoinmentsByUserId(int userId);

        //  GET BY ID
        Task<ApiResponse<PatientDto>> GetByIdAsync(int id);

        //  UPDATE PATIENT
        Task<ApiResponse<string>> UpdateAsync(int id, CreatePatientDto dto);

        //  DELETE (SOFT DELETE OPTIONAL)
        Task<ApiResponse<string>> DeleteAsync(int id);

        Task<ApiResponse<string>> ChangeStatusAsync(ChangePatientStatusDto dto);
        Task<ApiResponse<object>> GetAllWithPaginationAsync(PaginationRequestDto dto);
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PatientBookingSystem.Application.DTOs;
using PatientBookingSystem.Application.DTOs.Common;
using PatientBookingSystem.Application.Interfaces;

namespace PatientBookingSystem.API.Controllers
{
    [ApiController]
    [Route("api/patient")]
    public class PatientAppointmentController : ControllerBase
    {
        private readonly IPatientService _service;

        public PatientAppointmentController(IPatientService service)
        {
            _service = service;
        }

        // ✅ CREATE
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreatePatientDto dto)
        {
            try
            {
                var result = await _service.CreateAsync(dto);

                if (!result.IsSuccess)
                    return BadRequest(result);

                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(500, ApiResponse<string>.FailResponse("Something went wrong"));
            }
        }

        // ✅ GET ALL
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                return Ok(await _service.GetAllAsync());
            }
            catch (Exception)
            {
                return StatusCode(500, ApiResponse<string>.FailResponse("Something went wrong"));
            }
        }

        // ✅ GET ALL Appoinment By UserId
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetAppoinmentsByUserId(int userId)
        {
            try
            {
                var result = await _service.GetAppoinmentsByUserId(userId);

                if (!result.IsSuccess)
                    return NotFound(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>
                {
                    IsSuccess = false,
                    Message = "Something went wrong",
                    Data = ex.Message
                });
            }
        }

        // ✅ GET BY ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var result = await _service.GetByIdAsync(id);

                if (!result.IsSuccess)
                    return NotFound(result);

                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(500, ApiResponse<string>.FailResponse("Something went wrong"));
            }
        }

        // ✅ UPDATE
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] CreatePatientDto dto)
        {
            try
            {
                var result = await _service.UpdateAsync(id, dto);

                if (!result.IsSuccess)
                    return BadRequest(result);

                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(500, ApiResponse<string>.FailResponse("Something went wrong"));
            }
        }

        //// ✅ DELETE
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> Delete(int id)
        //{
        //    try
        //    {
        //        var result = await _service.DeleteAsync(id);

        //        if (!result.IsSuccess)
        //            return NotFound(result);

        //        return Ok(result);
        //    }
        //    catch (Exception)
        //    {
        //        return StatusCode(500, ApiResponse<string>.FailResponse("Something went wrong"));
        //    }
        //}

        // ✅ CHANGE STATUS (NEW API)
        [HttpPut("change-status")]
        public async Task<IActionResult> ChangeStatus(ChangePatientStatusDto dto)
        {
            try
            {
                var result = await _service.ChangeStatusAsync(dto);

                if (!result.IsSuccess)
                    return BadRequest(result);

                return Ok(result);
            }
            catch
            {
                return StatusCode(500, ApiResponse<string>.FailResponse("Something went wrong"));
            }
        }

        [HttpPost("list")]
        public async Task<IActionResult> GetAllWithPagination([FromBody] PaginationRequestDto dto)
        {
            try
            {
                var result = await _service.GetAllWithPaginationAsync(dto);

                if (!result.IsSuccess)
                    return BadRequest(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>
                {
                    IsSuccess = false,
                    Message = "Something went wrong",
                    Data = ex.Message
                });
            }
        }
    }
}


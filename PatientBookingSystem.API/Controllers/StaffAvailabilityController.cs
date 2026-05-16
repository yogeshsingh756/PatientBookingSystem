using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PatientBookingSystem.Application.DTOs;
using PatientBookingSystem.Application.DTOs.Common;
using PatientBookingSystem.Application.Interfaces;

namespace PatientBookingSystem.API.Controllers
{
    [ApiController]
    [Route("api/staff-availability")]
    [Authorize]
    public class StaffAvailabilityController : ControllerBase
    {
        private readonly IStaffAvailabilityService _service;

        public StaffAvailabilityController(IStaffAvailabilityService service)
        {
            _service = service;
        }

        // CREATE
        [HttpPost]
        public async Task<IActionResult> Create(CreateStaffAvailabilityDto dto)
        {
            try
            {
                var result = await _service.CreateAsync(dto);

                if (!result.IsSuccess)
                    return BadRequest(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>
                {
                    IsSuccess = false,
                    Message = "Something went wrong while creating availability",
                    Data = ex.Message // remove in production if needed
                });
            }
        }

        // GET BY STAFF ID
        [HttpGet("{staffId}")]
        public async Task<IActionResult> Get(int staffId)
        {
            try
            {
                var result = await _service.GetByStaffId(staffId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>
                {
                    IsSuccess = false,
                    Message = "Something went wrong while fetching availability",
                    Data = ex.Message
                });
            }
        }

        // GET BY Availibility ID
        [HttpGet("by-id/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var result = await _service.GetByIdAsync(id);

                if (!result.IsSuccess)
                    return NotFound(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>
                {
                    IsSuccess = false,
                    Message = "Something went wrong while fetching availability",
                    Data = ex.Message
                });
            }
        }

        // UPDATE
        [HttpPut]
        public async Task<IActionResult> Update(UpdateStaffAvailabilityDto dto)
        {
            try
            {
                var result = await _service.UpdateAsync(dto);

                if (!result.IsSuccess)
                    return BadRequest(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>
                {
                    IsSuccess = false,
                    Message = "Something went wrong while updating availability",
                    Data = ex.Message
                });
            }
        }

        // DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _service.DeleteAsync(id);

                if (!result.IsSuccess)
                    return BadRequest(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>
                {
                    IsSuccess = false,
                    Message = "Something went wrong while deleting availability",
                    Data = ex.Message
                });
            }
        }
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PatientBookingSystem.Application.DTOs;
using PatientBookingSystem.Application.DTOs.Common;
using PatientBookingSystem.Application.Interfaces;

namespace PatientBookingSystem.API.Controllers
{
    [ApiController]
    [Route("api/staff-availability")]
    public class StaffAvailabilityController : ControllerBase
    {
        private readonly IStaffAvailabilityService _service;

        public StaffAvailabilityController(IStaffAvailabilityService service)
        {
            _service = service;
        }

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
                return StatusCode(500, ApiResponse<string>.FailResponse(ex.Message));
            }
        }

        [HttpGet("{staffId}")]
        public async Task<IActionResult> Get(int staffId)
        {
            var result = await _service.GetByStaffId(staffId);
            return Ok(result);
        }
    }
}

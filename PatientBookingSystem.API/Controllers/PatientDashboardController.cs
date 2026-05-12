using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PatientBookingSystem.Application.DTOs.Common;
using PatientBookingSystem.Application.Interfaces;

namespace PatientBookingSystem.API.Controllers
{
    [Route("api/patient-dashboard")]
    [ApiController]
    [Authorize]
    public class PatientDashboardController : ControllerBase
    {
        private readonly IPatientDashboardService _service;

        public PatientDashboardController(IPatientDashboardService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetDashboard()
        {
            try
            {
                var result = await _service.GetDashboardAsync();

                if (!result.IsSuccess)
                    return BadRequest(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500,
                    ApiResponse<string>.FailResponse(ex.Message));
            }
        }
    }
}

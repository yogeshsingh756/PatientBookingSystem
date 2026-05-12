using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PatientBookingSystem.Application.DTOs.Common;
using PatientBookingSystem.Application.DTOs.Dashboard;
using PatientBookingSystem.Application.Interfaces;

namespace PatientBookingSystem.API.Controllers
{
    [ApiController]
    [Route("api/admin/dashboard")]
    public class AdminDashboardController : ControllerBase
    {
        private readonly IAdminDashboardService _service;

        public AdminDashboardController(IAdminDashboardService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetDashboard([FromQuery] DashboardFilterDto dto)
        {
            try
            {
                var result = await _service.GetDashboardAsync(dto);

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

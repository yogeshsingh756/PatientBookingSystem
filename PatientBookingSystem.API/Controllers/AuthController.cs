using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PatientBookingSystem.Application.DTOs;
using PatientBookingSystem.Application.DTOs.Common;
using PatientBookingSystem.Application.Interfaces;

namespace PatientBookingSystem.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequestDto dto)
        {
            try
            {
                var result = await _authService.SendOtpAsync(dto);

                if (!result.IsSuccess)
                    return BadRequest(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>
                {
                    IsSuccess = false,
                    Message = ex.Message
                });
            }
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp(VerifyOtpDto dto)
        {
            try
            {
                var result = await _authService.VerifyOtpAndRegisterAsync(dto);

                if (!result.IsSuccess)
                    return BadRequest(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>
                {
                    IsSuccess = false,
                    Message = ex.Message
                });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDto dto)
        {
            try
            {
                var result = await _authService.LoginAsync(dto);

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
                    Data = ex.Message // optional (remove in production)
                });
            }
        }

        [HttpPost("login/send-otp")]
        public async Task<IActionResult> SendLoginOtp(SendLoginOtpDto dto)
        {
            try
            {
                var result = await _authService.SendLoginOtpAsync(dto);

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

        [HttpPost("login/verify-otp")]
        public async Task<IActionResult> VerifyLoginOtp(VerifyLoginOtpDto dto)
        {
            try
            {
                var result = await _authService.VerifyLoginOtpAsync(dto);

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

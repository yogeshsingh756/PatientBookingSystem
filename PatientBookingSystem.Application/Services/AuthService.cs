using PatientBookingSystem.Application.DTOs;
using PatientBookingSystem.Application.DTOs.Common;
using PatientBookingSystem.Application.Interfaces;
using PatientBookingSystem.Application.Models;
using PatientBookingSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Net.WebRequestMethods;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PatientBookingSystem.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepo;
        private readonly INotificationService _notification;
        private readonly IOtpRepository _otpRepository;
        private readonly ITokenService _tokenService;

        public AuthService(
            IUserRepository userRepo,
            INotificationService notification,
            IOtpRepository otpRepository,
            ITokenService tokenService)
        {
            _userRepo = userRepo;
            _notification = notification;
            _otpRepository = otpRepository;
            _tokenService = tokenService;
        }

        public async Task<ApiResponse<string>> SendOtpAsync(RegisterRequestDto dto)
        {
            // Validate user not exists
            var exists = await _userRepo.ExistsAsync(dto.Email, dto.PhoneNumber);
            if (exists)
                return ApiResponse<string>.FailResponse("User already exists with this email or phone");

            // Generate OTP
            var otp = new Random().Next(100000, 999999).ToString();
            var otpEntity = new OtpVerification
            {
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                Otp = otp,

                Name = dto.Name,
                Password = dto.Password,
                Address = dto.Address,
                Landmark = dto.Landmark,
                HouseNumber = dto.HouseNumber,
                Role = dto.Role.ToLower(),

                ExpiryTime = DateTime.UtcNow.AddMinutes(5),
                IsUsed = false,
                CreatedAt = DateTime.UtcNow,
                PinCode = dto.PinCode
            };

            // Save OTP (temporary storage)
            await _otpRepository.SaveOtpAsync(otpEntity);

            // Send OTP
            //await _notification.SendSmsAsync(dto.PhoneNumber, $"Your OTP is {otp}");
            if (!string.IsNullOrEmpty(dto.Email))
            {
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await _notification.SendEmailAsync(
                            dto.Email,
                            "OTP Verification",
                            $"Your OTP is {otp}. OTP is valid for 5 minutes only."
                        );
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Email failed: " + ex.Message);
                    }
                });
            }
            return ApiResponse<string>.SuccessResponse("OTP sent successfully");
        
        }

        public async Task<ApiResponse<string>> VerifyOtpAndRegisterAsync(VerifyOtpDto dto)
        {
            var otpData = await _otpRepository.GetValidOtpAsync(dto.PhoneNumber, dto.Otp);

            if (otpData == null)
                return ApiResponse<string>.FailResponse("Invalid or expired OTP");
            // mark used
            await _otpRepository.MarkAsUsedAsync(otpData);

            var user = new User
            {
                Name = otpData.Name,
                Email = otpData.Email,
                PhoneNumber = otpData.PhoneNumber,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(otpData.Password),
                Address = otpData.Address,
                Landmark = otpData.Landmark,
                HouseNumber = otpData.HouseNumber,
                Role = otpData.Role,
                IsVerified = true,
                CreatedAt = DateTime.UtcNow,
                PinCode = otpData.PinCode,
                IsActive = true
            };
            // Send credentials
            await _userRepo.AddAsync(user);
            //await _notification.SendSmsAsync(dto.PhoneNumber, $"Login using Email Or Phone: {user.Email + "/" + user.PhoneNumber} Password: {data.Password}" +
            //$"Download The APP For Login Using Link : abc.com");
            if (!string.IsNullOrEmpty(user.Email))
            {
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await _notification.SendEmailAsync(user.Email,
               "Welcome To Patient APP",
               $"Login using Email Or Phone: {user.Email + "/" + user.PhoneNumber} Password: {otpData.Password}" +
               $"Download The APP For Login Using Link : abc.com");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Email failed: " + ex.Message);
                    }
                });
            }


            return ApiResponse<string>.SuccessResponse("User registered successfully");
        }

        public async Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginRequestDto dto)
        {
            var user = await _userRepo.GetByEmailPhoneAsync(dto.EmailOrPhone);

            if (user == null)
                return ApiResponse<LoginResponseDto>.FailResponse("User not found");


            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return ApiResponse<LoginResponseDto>.FailResponse("Invalid password");

            var token = _tokenService.GenerateToken(user);
            var response = new LoginResponseDto
            {
                Token = token,
                UserId = user.Id,
                Name = user.Name,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Role = user.Role,
                Address = user.Address,
                HouseNumber = user.HouseNumber,
                Landmark = user.Landmark,
                IsVerified = user.IsVerified,
                Pincode = user.PinCode
            };
            return ApiResponse<LoginResponseDto>.SuccessResponse(response, "Login successful");
        }

        public async Task<ApiResponse<string>> SendLoginOtpAsync(SendLoginOtpDto dto)
        {
            var user = await _userRepo.GetByEmailPhoneAsync(dto.PhoneNumber);

            if (user == null)
                return ApiResponse<string>.FailResponse("User not found");

            var otp = new Random().Next(100000, 999999).ToString();

            var otpEntity = new OtpVerification
            {
                PhoneNumber = dto.PhoneNumber,
                Otp = otp,
                ExpiryTime = DateTime.UtcNow.AddMinutes(5),
                IsUsed = false,
                CreatedAt = DateTime.UtcNow,
                Name = user.Name,
                Address = user.Address,
                Landmark = user.Landmark,
                HouseNumber = user.HouseNumber,
                Password = user.PasswordHash,
                Email = user.Email,
                Role = user.Role,
                PinCode = user.PinCode
            };

            await _otpRepository.SaveOtpAsync(otpEntity);

            //await _notification.SendSmsAsync(dto.PhoneNumber, $"Your login OTP is {otp}");
            if (!string.IsNullOrEmpty(user.Email))
            {
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await _notification.SendEmailAsync(
                user.Email,
                "Login OTP Verification",
                $"Your OTP is: {otp} This OTP is valid for 5 minutes.");
                }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Email failed: " + ex.Message);
                    }
                });
            }

            return ApiResponse<string>.SuccessResponse("OTP sent for login");
        }

        public async Task<ApiResponse<LoginResponseDto>> VerifyLoginOtpAsync(VerifyLoginOtpDto dto)
        {
            var otpData = await _otpRepository.GetValidOtpAsync(dto.PhoneNumber, dto.Otp);

            if (otpData == null)
                return ApiResponse<LoginResponseDto>.FailResponse("Invalid or expired OTP");

            await _otpRepository.MarkAsUsedAsync(otpData);

            var user = await _userRepo.GetByEmailPhoneAsync(dto.PhoneNumber);

            if (user == null)
                return ApiResponse<LoginResponseDto>.FailResponse("User not found");

            // TODO: Generate JWT
            var token = _tokenService.GenerateToken(user);
            var response = new LoginResponseDto
            {
                Token = token,
                UserId = user.Id,
                Name = user.Name,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Role = user.Role,
                Address = user.Address,
                HouseNumber = user.HouseNumber,
                Landmark = user.Landmark,
                IsVerified = user.IsVerified,
                                Pincode = user.PinCode
            };

            return ApiResponse<LoginResponseDto>.SuccessResponse(response, "Login successful via OTP");
        }
    }
}

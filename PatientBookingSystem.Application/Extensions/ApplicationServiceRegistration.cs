using Microsoft.Extensions.DependencyInjection;
using PatientBookingSystem.Application.Interfaces;
using PatientBookingSystem.Application.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace PatientBookingSystem.Application.Extensions
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IServiceService, ServiceService>();
            services.AddScoped<IPatientService, PatientService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IStaffService, StaffService>();
            // Register application services here
            return services;
        }
    }
}

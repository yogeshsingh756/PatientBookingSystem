using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PatientBookingSystem.Application.Interfaces;
using PatientBookingSystem.Infrastructure.Data;
using PatientBookingSystem.Infrastructure.Repositories;
using PatientBookingSystem.Infrastructure.Services;
using PatientBookingSystem.Infrastructure.Configurations;
using PatientBookingSystem.Application.DTOs.Common;


namespace PatientBookingSystem.Infrastructure.Extensions
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            // DB
            services.AddDbContext<AppDbContext>(options =>
                options.UseMySql(
                    config.GetConnectionString("DefaultConnection"),
                    ServerVersion.AutoDetect(config.GetConnectionString("DefaultConnection"))
                ));

            // Repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IOtpRepository, OtpRepository>();
            services.AddScoped<IServiceRepository, ServiceRepository>();
            services.AddScoped<IPatientRepository, PatientRepository>();
            services.AddScoped<IPatientStatusHistoryRepository, PatientStatusHistoryRepository>();
            services.AddScoped<IStaffRepository, StaffRepository>();
            services.AddScoped<IStaffDocumentRepository, StaffDocumentRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            // Services
            services.AddScoped<INotificationService, NotificationService>();
            // Settings
            services.Configure<EmailSettings>(config.GetSection("EmailSettings"));
            services.Configure<JwtSettings>(config.GetSection("JwtSettings"));

            return services;
        }
    }
}

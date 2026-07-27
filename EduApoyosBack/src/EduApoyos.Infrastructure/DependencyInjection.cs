using EduApoyos.Application.Interfaces;
using EduApoyos.Domain.Interfaces;
using EduApoyos.Infrastructure.Persistence;
using EduApoyos.Infrastructure.Repositories;
using EduApoyos.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EduApoyos.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ISupportRequestRepository, SupportRequestRepository>();
            services.AddScoped<IStudentRepository, StudentRepository>();
            services.AddScoped<IJwtTokenService, JwtTokenService>();
            services.AddScoped<IIdentityService, IdentityService>();

            return services;
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartCity.Application;

namespace SmartCity.Infrastructure
{
    public static class InfrastructureServiceExtension
    {
        public static IServiceCollection AddInfrastructureService(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
            configuration.GetConnectionString("DefaultConnection"),
            x => x.MigrationsAssembly("SmartCity.Infrastructure")));

            services.AddScoped<IRetailPropertyRepository, RetailPropertyRepository>();

            return services;
        }
    }
}
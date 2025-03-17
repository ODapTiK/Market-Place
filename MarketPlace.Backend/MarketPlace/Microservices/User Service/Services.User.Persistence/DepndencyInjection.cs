using Microsoft.Extensions.DependencyInjection;

namespace UserService
{
    public static class DepndencyInjection
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services)
        {
            services.AddScoped<IJwtOptions, JwtOptions>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IManufacturerRepository, ManufacturerRepository>();
            services.AddScoped<IAdminRepository, AdminRepository>();
            return services;
        }
    }
}

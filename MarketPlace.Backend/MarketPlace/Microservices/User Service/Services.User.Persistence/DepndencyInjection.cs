using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace UserService
{
    public static class DepndencyInjection
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IJwtOptions, JwtOptions>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IManufacturerRepository, ManufacturerRepository>();
            services.AddScoped<IAdminRepository, AdminRepository>();

            var connectionString = Environment.GetEnvironmentVariable("USER_HANGFIRE_DB_CONNECTION_STRING")
                ?? throw new InvalidOperationException("USER_HANGFIRE_DB_CONNECTION_STRING is not set in environment variables");
            services.AddEntityFrameworkNpgsql().AddDbContext<HangfireUserDbContext>(options =>
                options.UseNpgsql(connectionString));
            services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UsePostgreSqlStorage(connectionString));

            services.AddHangfireServer();

            return services;
        }
    }
}

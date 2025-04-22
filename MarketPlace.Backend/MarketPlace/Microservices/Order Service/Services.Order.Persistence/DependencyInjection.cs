using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace OrderService
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ICartRepository, CartRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IJwtOptions, JwtOptions>();

            var connectionString = Environment.GetEnvironmentVariable("ORDER_HANGFIRE_DB_CONNECTION_STRING") 
                ?? throw new InvalidOperationException("ORDER_HANGFIRE_DB_CONNECTION_STRING is not set in environment variables"); 
            services.AddDbContext<HangfireOrderDbContext>(options =>
               options.UseNpgsql(connectionString));
            services.AddHangfire(config => config
                .UsePostgreSqlStorage(connectionString));
            services.AddHangfireServer();

            return services;
        }
    }
}

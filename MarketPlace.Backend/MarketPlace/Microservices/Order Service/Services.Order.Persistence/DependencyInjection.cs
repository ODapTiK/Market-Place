using Microsoft.Extensions.DependencyInjection;

namespace OrderService
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services)
        {
            services.AddScoped<ICartRepository, CartRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IJwtOptions, JwtOptions>();
            return services;
        }
    }
}

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using StackExchange.Redis;

namespace ProductService
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IJwtOptions, JwtOptions>();
            services.AddScoped<IProductRepository, ProductRepository>();

            services.AddSingleton<IConnectionMultiplexer>(serviceProvider =>
            {
                var connectionString = configuration.GetConnectionString("Redis");
                return ConnectionMultiplexer.Connect(connectionString);
            });
            services.AddScoped<IProductRedisService>(provider =>
            {
                var connectionMultiplexer = provider.GetRequiredService<IConnectionMultiplexer>();
                return new ProductRedisService(connectionMultiplexer);
            });

            services.AddSingleton<IMongoClient>(serviceProvider =>
            {
                var connectionString = configuration.GetConnectionString("MongoDb");
                return new MongoClient(connectionString);
            });
            services.AddScoped<IProductDbContext>(provider =>
            {
                var mongoClient = provider.GetRequiredService<IMongoClient>();
                var databaseName = configuration["MongoDB:DatabaseName"];
                return new ProductDbContext(mongoClient, databaseName);
            });

            return services;
        }
    }
}

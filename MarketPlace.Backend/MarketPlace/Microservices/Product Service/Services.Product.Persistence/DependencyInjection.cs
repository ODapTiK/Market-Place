using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization;
using MongoDB.Bson;
using MongoDB.Driver;
using StackExchange.Redis;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.EntityFrameworkCore;

namespace ProductService
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            BsonSerializer.RegisterSerializer<Guid>(new GuidSerializer(GuidRepresentation.Standard));
            services.AddScoped<IJwtOptions, JwtOptions>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IBackgroundTaskService, BackgroundTaskService>();

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

            var connectionString = Environment.GetEnvironmentVariable("PRODUCT_HANGFIRE_DB_CONNECTION_STRING")
                ?? throw new InvalidOperationException("PRODUCT_HANGFIRE_DB_CONNECTION_STRING is not set in environment variables");
            services.AddDbContext<HangfireProductDbContext>(options =>
                options.UseNpgsql(connectionString));
            services.AddHangfire(config => config
                .UsePostgreSqlStorage(connectionString));
            services.AddHangfireServer();

            return services;
        }
    }
}

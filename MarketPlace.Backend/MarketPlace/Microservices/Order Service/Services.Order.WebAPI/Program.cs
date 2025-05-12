using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Proto.OrderProduct;
using Proto.OrderUser;
using Serilog;
using System.Reflection;
using System.Text;

namespace OrderService
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            DotNetEnv.Env.Load("../../../.env");
            var builder = WebApplication.CreateBuilder(args);
            var services = builder.Services;
            var configuration = builder.Configuration;

            configuration.AddEnvironmentVariables();

            var connectionString = Environment.GetEnvironmentVariable("ORDER_DB_CONNECTION_STRING")
                ?? throw new InvalidOperationException("ORDER_DB_CONNECTION_STRING is not set in environment variables");
            services.AddDbContext<OrderDbContext>(options =>
                options.UseNpgsql(connectionString, npgsqlOptionsAction =>
                    npgsqlOptionsAction.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorCodesToAdd: null)
                    ));
            services.AddScoped<IOrderDbContext>(provider => provider.GetService<OrderDbContext>());

            var jwtOptions = new JwtOptions()
            {
                ExpiredMinutes = int.Parse(Environment.GetEnvironmentVariable("JWT_EXPIRED_TIME") ?? throw new InvalidOperationException("JWT_EXPIRED_TIME is not set in environment variables")),
                Key = Environment.GetEnvironmentVariable("JWT_KEY") ?? throw new InvalidOperationException("JWT_KEY is not set in environment variables")
            };

            services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile(new AssemblyMappingProfile(Assembly.GetExecutingAssembly()));
                cfg.AddProfile(new AssemblyMappingProfile(typeof(IOrderDbContext).Assembly));
                cfg.AddProfile(new AssemblyMappingProfile(typeof(OrderDbContext).Assembly));
            });

            services.AddApplication(configuration);
            services.AddPersistence(configuration);

            services.AddHostedService<UpdateProductConsumer>();

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyHeader();
                    policy.AllowAnyMethod();
                    policy.AllowAnyOrigin();
                });
            });

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.IncludeErrorDetails = true;

                    options.TokenValidationParameters = new()
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key)),
                    };
                });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
                options.AddPolicy("User", policy => policy.RequireRole("User"));
                options.AddPolicy("Manufacturer", policy => policy.RequireRole("Manufacturer"));
            });

            services.AddGrpc();
            services.AddGrpcClient<OrderUserService.OrderUserServiceClient>(options =>
            {
                options.Address = new Uri("https://localhost:6012");
            }).ConfigurePrimaryHttpMessageHandler(() =>
            {
                var handler = new HttpClientHandler();

                handler.ServerCertificateCustomValidationCallback =
                   (sender, certificate, chain, sslPolicyErrors) => true;

                return handler;
            });
            services.AddGrpcClient<OrderProductService.OrderProductServiceClient>(options =>
            {
                options.Address = new Uri("https://localhost:6014");
            }).ConfigurePrimaryHttpMessageHandler(() =>
            {
                var handler = new HttpClientHandler();

                handler.ServerCertificateCustomValidationCallback =
                   (sender, certificate, chain, sslPolicyErrors) => true;

                return handler;
            });

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "MarketPlace v1", Version = "v1" });
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Description =
                        "JWT Authorization header using the Bearer scheme. \r\n\r\n" +
                        "Enter 'Bearer' [space] and the your token in the text input below. \r\n\r\n" +
                        "Example: \"Bearer 12casdc1sd\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Scheme = "Bearer"
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference()
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Scheme = "oauth2",
                        Name = "Bearer",
                        In = ParameterLocation.Header
                    },
                    new List<string>()
                    }

                });
            });
            services.AddControllers();

            services.AddHttpContextAccessor();

            LoggingService.Configure(configuration);
            builder.Host.UseSerilog();

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                try
                {
                    var context = serviceProvider.GetRequiredService<OrderDbContext>();
                    await context.Database.EnsureCreatedAsync();
                    var hangfireContext = serviceProvider.GetRequiredService<HangfireOrderDbContext>();
                    await hangfireContext.Database.EnsureCreatedAsync();
                }
                catch (Exception exception)
                {
                    Log.Fatal(exception, "An error occured while app initialization");
                }
            }

            app.UseExceptionHandler();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCors("AllowAll");
            app.UseSwagger();
            app.UseSwaggerUI(config =>
            {
                config.RoutePrefix = string.Empty;
                config.SwaggerEndpoint("/swagger/v1/swagger.json", "Event App API V1");
            });
            //app.UseHangfireServer();
            app.UseHangfireDashboard();

            app.MapControllers();
            app.MapGrpcService<UserServiceImpl>();
            app.MapGrpcService<ProductServiceImpl>();

            app.Run();
        }
    }
}

using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Proto.OrderUser;
using System.Reflection;
using System.Text;
using Serilog;
using Grpc.Core;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Server.Kestrel.Https;

namespace UserService
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

            var connectionString = Environment.GetEnvironmentVariable("USER_DB_CONNECTION_STRING")
                ?? throw new InvalidOperationException("USER_DB_CONNECTION_STRING is not set in environment variables");
            services.AddDbContext<UserDbContext>(options =>
                options.UseNpgsql(connectionString, npgsqlOptionsAction =>
                    npgsqlOptionsAction.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorCodesToAdd: null)
                    ));
            services.AddScoped<IUserDbContext>(provider => provider.GetService<UserDbContext>());

            var jwtOptions = new JwtOptions()
            {
                ExpiredMinutes = int.Parse(Environment.GetEnvironmentVariable("JWT_EXPIRED_TIME") ?? throw new InvalidOperationException("JWT_EXPIRED_TIME is not set in environment variables")),
                Key = Environment.GetEnvironmentVariable("JWT_KEY") ?? throw new InvalidOperationException("JWT_KEY is not set in environment variables")
            };

            services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile(new AssemblyMappingProfile(Assembly.GetExecutingAssembly()));
                cfg.AddProfile(new AssemblyMappingProfile(typeof(IUserDbContext).Assembly));
                cfg.AddProfile(new AssemblyMappingProfile(typeof(UserDbContext).Assembly));
                cfg.AddProfile(new AssemblyMappingProfile(typeof(AssemblyMappingProfile).Assembly));
            });

            services.AddApplication();
            services.AddPersistence(configuration);

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
                options.Address = new Uri("https://orderservice:6013");
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
                    var context = serviceProvider.GetRequiredService<UserDbContext>();
                    await context.Database.EnsureCreatedAsync();
                    var hangfireContext = serviceProvider.GetRequiredService<HangfireUserDbContext>();
                    await hangfireContext.Database.MigrateAsync();
                }
                catch (Exception exception)
                {
                    Log.Fatal(exception, "An error occured while app initialization");
                }
            }
            var webSocketOptions = new WebSocketOptions
            {
                KeepAliveInterval = TimeSpan.FromMinutes(2)
            };

            webSocketOptions.AllowedOrigins.Add("http://localhost:4200");

            app.UseWebSockets(webSocketOptions);

            app.UseExceptionHandler();
            app.UseRouting();
            app.UseCors("AllowAll");
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSwagger();
            app.UseSwaggerUI(config =>
            {
                config.RoutePrefix = string.Empty;
                config.SwaggerEndpoint("/swagger/v1/swagger.json", "Market Place API V1");
            });

            app.UseHangfireDashboard();

            var manager = new RecurringJobManager();
            manager.AddOrUpdate<BirthdayGreetingsGenerator>("birthday-greetings", x => x.GenerateBirthdayGreetings(default), Cron.Daily());

            app.MapControllers();
            app.MapGrpcService<AuthServiceImpl>();
            app.MapGrpcService<OrderServiceImpl>();
            app.MapGrpcService<ProductServiceImpl>();
            app.MapHub<NotificationHub>("/notificationHub");

            app.Run();
        }
    }
}

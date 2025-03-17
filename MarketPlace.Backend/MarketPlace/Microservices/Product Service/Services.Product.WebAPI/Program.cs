using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;

namespace ProductService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var services = builder.Services;
            var configuration = builder.Configuration;

            configuration.AddEnvironmentVariables();
            var jwtOptions = new JwtOptions()
            {
                ExpiredMinutes = int.Parse(Environment.GetEnvironmentVariable("JWT_EXPIRED_TIME") ?? throw new InvalidOperationException("JWT_EXPIRED_TIME is not set in environment variables")),
                Key = Environment.GetEnvironmentVariable("JWT_KEY") ?? throw new InvalidOperationException("JWT_KEY is not set in environment variables")
            };

            services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile(new AssemblyMappingProfile(Assembly.GetExecutingAssembly()));
                cfg.AddProfile(new AssemblyMappingProfile(typeof(IProductDbContext).Assembly));
                cfg.AddProfile(new AssemblyMappingProfile(typeof(ProductDbContext).Assembly));
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
            services.AddAuthorization();

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "MarketPlace v1", Version = "v1" });
            });
            services.AddControllers();

            services.AddHttpContextAccessor();

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                try
                {
                    var context = serviceProvider.GetRequiredService<ProductDbContext>();
                }
                catch (Exception exception)
                {
                    //Log.Fatal(exception, "An error occured while app initialization");
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
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.Run();
        }
    }
}

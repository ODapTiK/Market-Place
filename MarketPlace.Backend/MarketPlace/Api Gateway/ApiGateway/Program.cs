using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using MMLib.SwaggerForOcelot.DependencyInjection;
using Serilog;

namespace ApiGateway
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            DotNetEnv.Env.Load("../../.env");
            var builder = WebApplication.CreateBuilder(args);

            var services = builder.Services;
            var configuration = builder.Configuration;

            services.AddScoped<IJwtOptions, JwtOptions>();

            var jwtOptions = new JwtOptions()
            {
                ExpiredMinutes = int.Parse(Environment.GetEnvironmentVariable("JWT_EXPIRED_TIME") ?? throw new InvalidOperationException("JWT_EXPIRED_TIME is not set in environment variables")),
                Key = Environment.GetEnvironmentVariable("JWT_KEY") ?? throw new InvalidOperationException("JWT_KEY is not set in environment variables")
            };

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

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            configuration.AddOcelotWithSwaggerSupport((options) =>
            {
                options.Folder = "OcelotConfigurations";
                options.FileOfSwaggerEndPoints = "Ocelot.Swagger";
            });

            services.AddOcelot();
            services.AddSwaggerForOcelot(configuration);

            services.AddControllers();

            //services.AddHttpClient("SwaggerClient")
            //    .ConfigurePrimaryHttpMessageHandler(() => 
            //    {
            //        var handler = new HttpClientHandler();
            //        handler.ServerCertificateCustomValidationCallback = (msg, cert, chain, errors) =>
            //        {
            //            if (cert?.Subject.Contains("CN=localhost") ?? false) return true;
            //            return errors == System.Net.Security.SslPolicyErrors.None;
            //        };
            //        handler.SslProtocols = System.Security.Authentication.SslProtocols.Tls12 |
            //                              System.Security.Authentication.SslProtocols.Tls13;

            //        return handler;
            //    })
            //    .ConfigureHttpClient(client =>
            //    {
            //        client.Timeout = TimeSpan.FromSeconds(30);
            //    });

            LoggingService.Configure(configuration);
            builder.Host.UseSerilog();

            var app = builder.Build();

            app.UseCors("AllowAll");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/docs/v1/aggregated", "API Gateway");
                c.RoutePrefix = string.Empty;
            });

            app.UseSwaggerForOcelotUI(options =>
            {
                options.PathToSwaggerGenerator = "/swagger/docs";
            });

            await app.UseOcelot();

            app.MapControllers();

            app.Run();
        }
    }
}

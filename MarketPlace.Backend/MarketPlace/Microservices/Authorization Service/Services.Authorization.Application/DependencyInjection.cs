using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace AuthorizationService
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddValidatorsFromAssemblies([Assembly.GetExecutingAssembly()]);
            services.AddMediatR(config => config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

            services.AddScoped<IPasswordEncryptor, PasswordEncryptor>();
            services.AddScoped<IJwtProvider, JwtProvider>();
            services.AddScoped<IJwtOptions, JwtOptions>();


            return services;
        }
    }
}

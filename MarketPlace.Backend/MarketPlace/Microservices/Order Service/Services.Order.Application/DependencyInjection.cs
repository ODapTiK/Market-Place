﻿using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace OrderService
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMediatR(config => config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            services.AddScoped<IObsoleteOrderCollector, ObsoleteOrdersCollector>();
            services.AddScoped<IRabbitMqOptions,  RabbitMqOptions>();
            services.Configure<RabbitMqOptions>(configuration.GetSection(nameof(RabbitMqOptions)));
            services.AddValidatorsFromAssemblies([Assembly.GetExecutingAssembly()]);
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            return services;
        }
    }
}

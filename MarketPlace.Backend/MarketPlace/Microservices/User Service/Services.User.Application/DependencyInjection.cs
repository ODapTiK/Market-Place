using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.Extensions.Configuration;

namespace UserService
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddValidatorsFromAssemblies([Assembly.GetExecutingAssembly()]);

            services.AddScoped<ICreateAdminUseCase, CreateAdminUseCase>();
            services.AddScoped<IUpdateAdminUseCase, UpdateAdminUseCase>();
            services.AddScoped<IDeleteAdminUseCase, DeleteAdminUseCase>();
            services.AddScoped<IGetAdminInfoUseCase, GetAdminInfoUseCase>();

            services.AddScoped<ICreateManufacturerUseCase, CreateManufacturerUseCase>();
            services.AddScoped<IUpdateManufacturerUseCase, UpdateManufacturerUseCase>();
            services.AddScoped<IDeleteManufacturerUseCase, DeleteManufacturerUseCase>();
            services.AddScoped<IGetManufacturerInfoUseCase, GetManufacturerInfoUseCase>();
            services.AddScoped<IAddManufacturerProductUseCase, AddManufacturerProductUseCase>();
            services.AddScoped<IRemoveManufacturerProductUseCase, RemoveManufacturerProductUseCase>();
            services.AddScoped<IGetManufacturersIdUseCase, GetManufacturersIdUseCase>();

            services.AddScoped<ICreateUserUseCase, CreateUserUseCase>();
            services.AddScoped<IUpdateUserUseCase, UpdateUserUseCase>();
            services.AddScoped<IDeleteUserUseCase, DeleteUserUseCase>();
            services.AddScoped<IGetUserInfoUseCase, GetUserInfoUseCase>();
            services.AddScoped<IAddUserOrderUseCase, AddUserOrderUseCase>();
            services.AddScoped<IRemoveUserOrderUseCase, RemoveUserOrderUseCase>();
            services.AddScoped<IGetUsersWithBirthdayUseCase, GetUsersWithBirthdayUseCase>();

            services.AddHangfire(config => config
               .UseSimpleAssemblyNameTypeSerializer()
               .UseRecommendedSerializerSettings()
               .UsePostgreSqlStorage(configuration.GetConnectionString("HangfireDB")));
            services.AddHangfireServer();

            var manager = new RecurringJobManager();
            manager.AddOrUpdate<BirthdayGreetingsGenerator>("birthday-greetings", x => x.GenerateBirthdayGreetings(default), Cron.Daily);

            return services;
        }
    }
}

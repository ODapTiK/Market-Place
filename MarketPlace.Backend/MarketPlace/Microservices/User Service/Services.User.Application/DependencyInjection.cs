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
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddValidatorsFromAssemblies([Assembly.GetExecutingAssembly()]);
            services.AddSignalR();

            services.AddScoped<ICreateAdminUseCase, CreateAdminUseCase>();
            services.AddScoped<IUpdateAdminUseCase, UpdateAdminUseCase>();
            services.AddScoped<IUpdateAdminLogoUseCase, UpdateAdminLogoUseCase>();
            services.AddScoped<IDeleteAdminUseCase, DeleteAdminUseCase>();
            services.AddScoped<IGetAdminInfoUseCase, GetAdminInfoUseCase>();
            services.AddScoped<IGetAllAdminsUseCase, GetAllAdminsUseCase>();
            services.AddScoped<IAddOrderToControlAdminUseCase, AddOrderToControlAdminUseCase>();
            services.AddScoped<IRemoveControlAdminOrderUseCase, RemoveControlAdminOrderUseCase>();
            services.AddScoped<IReadAdminNotificationUseCase, ReadAdminNotificationUseCase>();
            services.AddScoped<IGetAdminUnreadNotificationsCountUseCase, GetAdminUnreadNotificationsCountUseCase>();
            services.AddScoped<IAddAdminNotificationUseCase,  AddAdminNotificationUseCase>();

            services.AddScoped<ICreateManufacturerUseCase, CreateManufacturerUseCase>();
            services.AddScoped<IUpdateManufacturerUseCase, UpdateManufacturerUseCase>();
            services.AddScoped<IUpdateManufacturerLogoUseCase, UpdateManufacturerLogoUseCase>();
            services.AddScoped<IDeleteManufacturerUseCase, DeleteManufacturerUseCase>();
            services.AddScoped<IGetManufacturerInfoUseCase, GetManufacturerInfoUseCase>();
            services.AddScoped<IAddManufacturerProductUseCase, AddManufacturerProductUseCase>();
            services.AddScoped<IRemoveManufacturerProductUseCase, RemoveManufacturerProductUseCase>();
            services.AddScoped<IGetManufacturersIdUseCase, GetManufacturersIdUseCase>();
            services.AddScoped<IGetManufacturerUnreadNotificationsCountUseCase, GetManufacturerUnreadNotificationsCountUseCase>();
            services.AddScoped<IReadManufacturerNotificationUseCase, ReadManufacturerNotificationUseCase>();
            services.AddScoped<IAddManufacturerNotificationUseCase, AddManufacturerNotificationUseCase>();

            services.AddScoped<ICreateUserUseCase, CreateUserUseCase>();
            services.AddScoped<IUpdateUserUseCase, UpdateUserUseCase>();
            services.AddScoped<IUpdateUserLogoUseCase, UpdateUserLogoUseCase>();
            services.AddScoped<IDeleteUserUseCase, DeleteUserUseCase>();
            services.AddScoped<IGetUserInfoUseCase, GetUserInfoUseCase>();
            services.AddScoped<IAddUserOrderUseCase, AddUserOrderUseCase>();
            services.AddScoped<IRemoveUserOrderUseCase, RemoveUserOrderUseCase>();
            services.AddScoped<IGetUsersWithBirthdayUseCase, GetUsersWithBirthdayUseCase>();
            services.AddScoped<IGetUserUnreadNotificationsCountUseCase, GetUserUreadNotificationsCountUseCase>();
            services.AddScoped<IAddUserNotificationUseCase, AddUserNotificationUseCase>();
            services.AddScoped<IReadUserNotificationUseCase, ReadUserNotificationUseCase>();


            return services;
        }
    }
}

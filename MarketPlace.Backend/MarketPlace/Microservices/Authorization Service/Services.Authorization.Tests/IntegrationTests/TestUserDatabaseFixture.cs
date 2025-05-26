using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;

namespace AuthorizationService
{
    public class TestUserDatabaseFixture : IAsyncLifetime
    {
        private readonly PostgreSqlContainer _postgreSqlContainer;
        private IServiceProvider _serviceProvider;
        private IServiceScope _scope;

        public AuthDbContext _context => _scope.ServiceProvider.GetRequiredService<AuthDbContext>();
        public UserManager<User> _userManager => _scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        public RoleManager<IdentityRole<Guid>> _roleManager => _scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

        public TestUserDatabaseFixture()
        {
            _postgreSqlContainer = new PostgreSqlBuilder()
                .WithImage("postgres:15-alpine")
                .WithDatabase("test_auth_db")
                .WithUsername("postgres")
                .WithPassword("postgres")
                .WithCleanUp(true)
                .Build();
        }

        public async Task InitializeAsync()
        {
            await _postgreSqlContainer.StartAsync();

            var services = new ServiceCollection();

            services.AddDbContext<AuthDbContext>(options =>
                options.UseNpgsql(_postgreSqlContainer.GetConnectionString(),
                    npgsqlOptions => npgsqlOptions.MigrationsAssembly(typeof(AuthDbContext).Assembly.FullName)),
                ServiceLifetime.Scoped);

            services.AddIdentity<User, IdentityRole<Guid>>()
                .AddEntityFrameworkStores<AuthDbContext>()
                .AddDefaultTokenProviders();

            services.AddLogging();

            _serviceProvider = services.BuildServiceProvider();
            _scope = _serviceProvider.CreateScope();

            // Применяем миграции
            await _context.Database.MigrateAsync();

            // Создаем роль Admin
            await _roleManager.CreateAsync(new IdentityRole<Guid> { Name = "Admin" });
        }

        public async Task DisposeAsync()
        {
            if (_scope != null)
            {
                await _context.Database.EnsureDeletedAsync();
                _scope.Dispose();
            }

            await _postgreSqlContainer.DisposeAsync();
        }
    }
}

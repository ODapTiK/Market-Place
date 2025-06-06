using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;

namespace UserService
{
    public class TestUserDatabaseFixture : IAsyncLifetime
    {
        private readonly PostgreSqlContainer _postgreSqlContainer;
        private IServiceProvider _serviceProvider;
        private IServiceScope _scope;

        public UserDbContext _context => _scope.ServiceProvider.GetRequiredService<UserDbContext>();

        public TestUserDatabaseFixture()
        {
            // Настройка контейнера PostgreSQL
            _postgreSqlContainer = new PostgreSqlBuilder()
                .WithImage("postgres:15-alpine")
                .WithDatabase("test_user_db")
                .WithUsername("postgres")
                .WithPassword("postgres")
                .WithCleanUp(true)
                .Build();
        }

        public async Task InitializeAsync()
        {
            // Запуск контейнера
            await _postgreSqlContainer.StartAsync();

            var services = new ServiceCollection();

            // Настройка DbContext с подключением к контейнеру
            services.AddDbContext<UserDbContext>(options =>
                options.UseNpgsql(_postgreSqlContainer.GetConnectionString(),
                    npgsqlOptions => npgsqlOptions.MigrationsAssembly(typeof(UserDbContext).Assembly.FullName)),
                ServiceLifetime.Scoped);

            _serviceProvider = services.BuildServiceProvider();
            _scope = _serviceProvider.CreateScope();

            // Применение миграций
            await _context.Database.MigrateAsync();
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

        // Дополнительный метод для создания новых scope (опционально)
        public IServiceScope CreateScope() => _serviceProvider.CreateScope();
    }
}

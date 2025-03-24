using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AuthorizationService
{
    public class TestUserDatabaseFixture : IDisposable
    {
        public AuthDbContext _context { get; private set; }
        private readonly IServiceScope _scope;

        public TestUserDatabaseFixture()
        {
            var services = new ServiceCollection();

            services.AddDbContext<AuthDbContext>(options =>
                options.UseInMemoryDatabase("TestDatabase"));

            var serviceProvider = services.BuildServiceProvider();
            serviceProvider.CreateScope();
            _context = serviceProvider.GetRequiredService<AuthDbContext>();
        }
        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
            _scope?.Dispose();
        }
    }
}

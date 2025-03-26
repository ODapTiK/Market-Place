using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace UserService
{
    public class TestUserDatabaseFixture : IDisposable
    {
        public UserDbContext _context { get; private set; }
        private readonly IServiceScope _scope;

        public TestUserDatabaseFixture()
        {
            var services = new ServiceCollection();

            services.AddDbContext<UserDbContext>(options =>
                options.UseInMemoryDatabase("TestDatabase"));

            var serviceProvider = services.BuildServiceProvider();
            _scope = serviceProvider.CreateScope();
            _context = serviceProvider.GetRequiredService<UserDbContext>();
        }
        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
            _scope?.Dispose();
        }
    }
}

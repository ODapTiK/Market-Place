using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace OrderService
{
    public class TestOrderDatabaseFixture : IDisposable
    {
        public OrderDbContext _context { get; private set; }
        private readonly IServiceScope _scope;

        public TestOrderDatabaseFixture()
        {
            var services = new ServiceCollection();

            services.AddDbContext<OrderDbContext>(options =>
                options.UseInMemoryDatabase("TestDatabase"));

            var serviceProvider = services.BuildServiceProvider();
            _scope = serviceProvider.CreateScope();
            _context = serviceProvider.GetRequiredService<OrderDbContext>();
        }
        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
            _scope?.Dispose();
        }
    }
}

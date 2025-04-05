using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace OrderService
{
    public class TestOrderDatabaseFixture : IDisposable
    {
        public OrderDbContext _context { get; private set; }
        private readonly IServiceScope _scope;
        private bool _isDisposed = false;

        public TestOrderDatabaseFixture()
        {
            if(_isDisposed)
                throw new ObjectDisposedException(ToString());

            var services = new ServiceCollection();

            services.AddDbContext<OrderDbContext>(options =>
                options.UseInMemoryDatabase("TestDatabase"));

            var serviceProvider = services.BuildServiceProvider();
            _scope = serviceProvider.CreateScope();
            _context = serviceProvider.GetRequiredService<OrderDbContext>();
        }
        public void Dispose()
        {
            if (!_isDisposed)
            {
                _isDisposed = true;
                _context.Database.EnsureDeleted();
                _context.Dispose();
                _scope?.Dispose();
            }
        }
    }
}

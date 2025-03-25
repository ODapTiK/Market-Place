using Microsoft.EntityFrameworkCore;

namespace OrderService
{
    public interface IOrderDbContext
    {
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderPoint> OrderPoints { get; set; }
        public DbSet<Cart> Carts { get; set; }

        DbSet<T> Set<T>() where T : class;
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}

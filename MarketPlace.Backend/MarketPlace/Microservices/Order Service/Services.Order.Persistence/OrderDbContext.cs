using Microsoft.EntityFrameworkCore;

namespace OrderService
{
    public class OrderDbContext : DbContext, IOrderDbContext
    {
        public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options) { }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderPoint> OrderPoints { get; set; }
        public DbSet<Cart> Carts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new OrderConfiguration());
            modelBuilder.ApplyConfiguration(new CartConfiguration());   
            modelBuilder.ApplyConfiguration(new OrderPointConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}

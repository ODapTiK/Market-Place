using Microsoft.EntityFrameworkCore;

namespace ProductService
{
    public class HangfireProductDbContext : DbContext
    {
        public HangfireProductDbContext(DbContextOptions<HangfireProductDbContext> options) : base(options) { }
    }
}

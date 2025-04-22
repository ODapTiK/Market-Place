using Microsoft.EntityFrameworkCore;

namespace OrderService
{
    public class HangfireOrderDbContext : DbContext
    {
        public HangfireOrderDbContext(DbContextOptions<HangfireOrderDbContext> options) : base(options) { }
    }
}

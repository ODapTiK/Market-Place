using Microsoft.EntityFrameworkCore;

namespace UserService
{
    public class HangfireUserDbContext : DbContext
    {
        public HangfireUserDbContext(DbContextOptions<HangfireUserDbContext> options) : base(options) { }   
    }
}

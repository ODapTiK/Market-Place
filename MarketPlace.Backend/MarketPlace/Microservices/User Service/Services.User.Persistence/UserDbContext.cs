using Microsoft.EntityFrameworkCore;

namespace UserService
{
    public class UserDbContext : DbContext, IUserDbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Manufacturer> Manufacturers { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new AdminConfiguration());
            modelBuilder.ApplyConfiguration(new ManufacturerConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}

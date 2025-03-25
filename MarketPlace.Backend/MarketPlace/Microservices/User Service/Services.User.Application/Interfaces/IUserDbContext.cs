using Microsoft.EntityFrameworkCore;

namespace UserService
{
    public interface IUserDbContext  
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Manufacturer> Manufacturers { get; set; }

        DbSet<T> Set<T>() where T : class;
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}

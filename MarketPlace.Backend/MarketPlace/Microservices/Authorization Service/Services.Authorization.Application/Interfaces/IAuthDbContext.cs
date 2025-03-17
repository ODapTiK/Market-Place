using Microsoft.EntityFrameworkCore;

namespace AuthorizationService
{
    public interface IAuthDbContext
    {
        public DbSet<User> Users { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}

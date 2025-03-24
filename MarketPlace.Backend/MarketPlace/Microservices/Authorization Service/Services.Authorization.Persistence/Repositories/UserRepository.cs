using Microsoft.EntityFrameworkCore;

namespace AuthorizationService
{
    public class UserRepository : IUserRepository
    {
        private readonly IAuthDbContext _authDbContext;

        public UserRepository(IAuthDbContext authDbContext)
        {
            _authDbContext = authDbContext;
        }

        public async Task<Guid> CreateAsync(User user, CancellationToken cancellationToken)
        {
            await _authDbContext.Users.AddAsync(user);
            await _authDbContext.SaveChangesAsync(cancellationToken);

            return user.Id;
        }

        public async Task DeleteAsync(User user, CancellationToken cancellationToken)
        {
            _authDbContext.Users.Remove(user);
            await _authDbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<User?> FindByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var user = await _authDbContext.Users.FindAsync([id], cancellationToken);

            return user;
        }

        public async Task<User?> FindByEmailAsync(string email, CancellationToken cancellationToken)
        {
            var user = await _authDbContext.Users.FirstOrDefaultAsync(x => x.Email.Equals(email), cancellationToken);

            return user;
        }

        public async Task UpdateAsync(CancellationToken cancellationToken)
        {
            await _authDbContext.SaveChangesAsync(cancellationToken);
        }
    }
}

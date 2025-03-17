namespace AuthorizationService
{
    public interface IUserRepository
    {
        public Task<Guid> CreateAsync(User user, CancellationToken cancellationToken);
        public Task DeleteAsync(User user, CancellationToken cancellationToken);
        public Task<User?> FindByIdAsync(Guid id, CancellationToken cancellationToken);
        public Task<User?> FindByEmailAsync(string email, CancellationToken cancellationToken);
        public Task UpdateAsync(CancellationToken cancellationToken);
    }
}

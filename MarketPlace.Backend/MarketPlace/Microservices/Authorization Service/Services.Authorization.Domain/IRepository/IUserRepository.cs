using Microsoft.AspNetCore.Identity;

namespace AuthorizationService
{
    public interface IUserRepository
    {
        Task<Guid> CreateAsync(User user, CancellationToken cancellationToken);
        Task DeleteAsync(User user, CancellationToken cancellationToken);
        Task<User?> FindByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<User?> FindByEmailAsync(string email, CancellationToken cancellationToken);
        Task UpdateAsync(User user, CancellationToken cancellationToken);
        Task<List<string>> GetUserRoleAsync(User user);
        Task<IdentityResult> AddUserToRoleAsync(User user, string roleName);
    }
}

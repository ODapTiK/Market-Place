using Microsoft.AspNetCore.Identity;

namespace AuthorizationService
{
    public interface IRoleRepository
    {
        Task<IdentityRole<Guid>?> GetRoleByNameAsync(string roleName);
        Task<bool> RoleExistsAsync(string roleName);
        Task<IdentityResult> CreateRoleAsync(IdentityRole<Guid> role);
    }
}

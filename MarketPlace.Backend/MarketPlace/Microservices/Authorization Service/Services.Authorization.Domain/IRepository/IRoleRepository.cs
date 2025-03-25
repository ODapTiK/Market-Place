using Microsoft.AspNetCore.Identity;

namespace AuthorizationService
{
    public interface IRoleRepository
    {
        Task<IdentityRole?> GetRoleByNameAsync(string roleName);
        Task<bool> RoleExistsAsync(string roleName);
        Task<IdentityResult> CreateRoleAsync(IdentityRole role);
    }
}

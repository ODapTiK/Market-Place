using Microsoft.AspNetCore.Identity;

namespace AuthorizationService
{
    public class RoleRepository : IRoleRepository
    {
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;

        public RoleRepository(RoleManager<IdentityRole<Guid>> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<IdentityRole<Guid>?> GetRoleByNameAsync(string roleName)
        {
            return await _roleManager.FindByNameAsync(roleName);
        }

        public async Task<bool> RoleExistsAsync(string roleName)
        {
            return await _roleManager.RoleExistsAsync(roleName);
        }

        public async Task<IdentityResult> CreateRoleAsync(IdentityRole<Guid> role)
        {
            return await _roleManager.CreateAsync(role);
        }
    }
}

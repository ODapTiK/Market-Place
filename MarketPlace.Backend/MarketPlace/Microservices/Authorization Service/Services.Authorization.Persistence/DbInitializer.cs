using Microsoft.AspNetCore.Identity;

namespace AuthorizationService
{
    public class DbInitializer
    {
        private readonly IRoleRepository _roleRepository;

        public DbInitializer(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task InitializeAsync()
        {
            var roles = Enum.GetValues(typeof(Role));
            foreach (var role in roles)
            {
                var roleName = role.ToString();
                if (!await _roleRepository.RoleExistsAsync(roleName))
                {
                    await _roleRepository.CreateRoleAsync(new IdentityRole<Guid>(roleName));
                }
            }
        }
    }
}

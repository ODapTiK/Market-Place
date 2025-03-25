using Microsoft.AspNetCore.Identity;

namespace AuthorizationService
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<User> _userManager;

        public UserRepository(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<Guid> CreateAsync(User user, CancellationToken cancellationToken)
        {
            var result = await _userManager.CreateAsync(user, user.PasswordHash);
            if (result.Succeeded)
            {
                return user.Id;
            }
            throw new Exception("User  creation failed: " + string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        public async Task DeleteAsync(User user, CancellationToken cancellationToken)
        {
            await _userManager.DeleteAsync(user);
        }

        public async Task<User?> FindByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _userManager.FindByIdAsync(id.ToString());
        }

        public async Task<User?> FindByEmailAsync(string email, CancellationToken cancellationToken)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task UpdateAsync(User user, CancellationToken cancellationToken)
        {
            await _userManager.UpdateAsync(user);
        }
        public async Task<IdentityResult> AddUserToRoleAsync(User user, string roleName)
        {
            return await _userManager.AddToRoleAsync(user, roleName);
        }

        public async Task<List<string>> GetUserRoleAsync(User user)
        {
            return (List<string>)await _userManager.GetRolesAsync(user);
        }
    }
}

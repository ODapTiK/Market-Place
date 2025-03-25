using Microsoft.AspNetCore.Identity;

namespace AuthorizationService
{
    public class User : IdentityUser<Guid>
    {
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime RefreshTokenExpiryTime { get; set; }
    }
}

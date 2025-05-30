﻿using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AuthorizationService
{
    public class JwtProvider : IJwtProvider
    {
        private readonly IJwtOptions _Options;
        private readonly IUserRepository _UserRepository;
        public JwtProvider(IJwtOptions options, IUserRepository userRepository)
        {
            _Options = options;
            _UserRepository = userRepository;
        }
        private async Task<string> GenerateAccessToken(User user)
        {
            var signinCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_Options.Key)),
            SecurityAlgorithms.HmacSha256);

            var roles = await _UserRepository.GetUserRoleAsync(user); 
            var claims = new List<Claim>
            {
                new Claim("UserId", user.Id.ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var token = new JwtSecurityToken(
                signingCredentials: signinCredentials,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_Options.ExpiredMinutes));

            var tokenValue = new JwtSecurityTokenHandler().WriteToken(token);
            return tokenValue;
        }

        private string GenerateRefreshToken()
        {
            var random = new byte[32];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(random);

                return Convert.ToBase64String(random);
            }
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_Options.Key)),
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;

            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);

            var jwtSecurityToken = securityToken as JwtSecurityToken;

            if (jwtSecurityToken is null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token message!");

            return principal;
        }

        public async Task<TokenDTO> GenerateToken(User user, bool populateExp, CancellationToken cancellationToken)
        {
            var accessToken = await GenerateAccessToken(user);
            var refreshToken = GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            if (populateExp)
            {
                user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            }
            await _UserRepository.UpdateAsync(user, cancellationToken);
            var userRole = (await _UserRepository.GetUserRoleAsync(user)).FirstOrDefault();

            return new TokenDTO(accessToken, refreshToken, userRole);
        }

        public async Task<TokenDTO> RefreshToken(TokenDTO tokenDto)
        {
            var principal = GetPrincipalFromExpiredToken(tokenDto.accessToken);

            var userId = principal.Claims.FirstOrDefault(c => c.Type.Equals("UserId"))?.Value ?? throw new InvalidAccessTokenException(nameof(ClaimTypes), "UserId");

            User user = await _UserRepository.FindByIdAsync(Guid.Parse(userId), CancellationToken.None) ?? throw new EntityNotFoundException(nameof(User), userId);

            if (user.RefreshToken != tokenDto.refreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                throw new RefreshTokenBadRequestException();
            }

            return await GenerateToken(user, false, CancellationToken.None);
        }
    }
}


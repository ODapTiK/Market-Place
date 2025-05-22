using FluentAssertions;
using Moq;

namespace AuthorizationService
{
    public class JwtProviderTests
    {
        private readonly JwtProvider _jwtProvider;
        private readonly Mock<IJwtOptions> _mockJwtOptions;
        private readonly Mock<IUserRepository> _mockUserRepository;

        public JwtProviderTests()
        {
            _mockJwtOptions = new Mock<IJwtOptions>();
            _mockUserRepository = new Mock<IUserRepository>();
            _mockJwtOptions.Setup(o => o.Key).Returns("asdfuAFauhddAFKJFnviuhSKVJSNDFakfnKSJDFNakfbfjnAsfasf");
            _mockJwtOptions.Setup(o => o.ExpiredMinutes).Returns(30);

            _jwtProvider = new JwtProvider(_mockJwtOptions.Object, _mockUserRepository.Object);
        }


        [Fact]
        public async Task GenerateToken_ShouldReturnTokenDTO()
        {
            // Arrange
            var user = new User 
            { 
                Id = Guid.NewGuid(),  
            };

            _mockUserRepository.Setup(repo => repo.GetUserRoleAsync(It.IsAny<User>())).ReturnsAsync(["Admin"]);

            // Act
            var tokenDto = await _jwtProvider.GenerateToken(user, true, CancellationToken.None);

            // Assert
            tokenDto.Should().NotBeNull();
            tokenDto.accessToken.Should().NotBeEmpty();
            tokenDto.refreshToken.Should().NotBeEmpty();
            user.RefreshToken.Should().Be(tokenDto.refreshToken);
            _mockUserRepository.Verify(repo => repo.UpdateAsync(user, CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task RefreshToken_ShouldReturnNewTokenDTO()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();

            var user = new User
            {
                Id = Guid.Parse(userId),
            };

            _mockUserRepository.Setup(repo => repo.GetUserRoleAsync(It.IsAny<User>())).ReturnsAsync(["Admin"]);
            _mockUserRepository.Setup(repo => repo.FindByIdAsync(It.IsAny<Guid>(), CancellationToken.None)).ReturnsAsync(user);

            var token = await _jwtProvider.GenerateToken(user, true, CancellationToken.None);

            // Act
            await Task.Delay(1000);
            var newTokenDto = await _jwtProvider.RefreshToken(token);

            // Assert
            newTokenDto.Should().NotBeNull();
            newTokenDto.accessToken.Should().NotBeEmpty();
            newTokenDto.refreshToken.Should().NotBeEmpty();
            newTokenDto.refreshToken.Should().NotBe(token.refreshToken);
            newTokenDto.accessToken.Should().NotBe(token.accessToken);
        }

        [Fact]
        public async Task RefreshToken_ShouldThrowException_WhenRefreshTokenIsInvalid()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var user = new User
            {
                Id = Guid.Parse(userId),
                RefreshToken = "validRefreshToken",
                RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(1)
            };

            _mockUserRepository.Setup(repo => repo.GetUserRoleAsync(It.IsAny<User>())).ReturnsAsync(["Admin"]);
            _mockUserRepository.Setup(repo => repo.FindByIdAsync(It.IsAny<Guid>(), CancellationToken.None)).ReturnsAsync(user);

            var expiredToken = await _jwtProvider.GenerateToken(user, false, CancellationToken.None);
            var tokenDto = new TokenDTO(expiredToken.accessToken, "invalidRefreshToken", "User");

            // Act 
            var act = async () => await _jwtProvider.RefreshToken(tokenDto);

            //Assert
            await act.Should().ThrowAsync<RefreshTokenBadRequestException>();
        }

        [Fact]
        public async Task RefreshToken_ShouldThrowException_WhenRefreshTokenExpired()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var user = new User
            {
                Id = Guid.Parse(userId),
                RefreshToken = "validRefreshToken",
                //expired
                RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(-1) 
            };

            _mockUserRepository.Setup(repo => repo.GetUserRoleAsync(It.IsAny<User>())).ReturnsAsync(["Admin"]);
            _mockUserRepository.Setup(repo => repo.FindByIdAsync(It.IsAny<Guid>(), CancellationToken.None)).ReturnsAsync(user);

            var expiredToken = await _jwtProvider.GenerateToken(user, false, CancellationToken.None);
            var tokenDto = new TokenDTO(expiredToken.accessToken, "validRefreshToken", "Admin");

            // Act 
            var act = async () => await _jwtProvider.RefreshToken(tokenDto);

            //Assert
            await act.Should().ThrowAsync<RefreshTokenBadRequestException>();
        }
    }
}

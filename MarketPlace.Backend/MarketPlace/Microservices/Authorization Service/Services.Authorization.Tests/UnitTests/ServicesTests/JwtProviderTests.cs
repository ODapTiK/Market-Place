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
                Role = "Admin" 
            };

            // Act
            var tokenDto = await _jwtProvider.GenerateToken(user, true, CancellationToken.None);

            // Assert
            tokenDto.Should().NotBeNull();
            tokenDto.accessToken.Should().NotBeEmpty();
            tokenDto.refreshToken.Should().NotBeEmpty();
            user.RefreshToken.Should().Be(tokenDto.refreshToken);
            _mockUserRepository.Verify(repo => repo.UpdateAsync(CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task RefreshToken_ShouldReturnNewTokenDTO()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();

            var user = new User
            {
                Id = Guid.Parse(userId),
                Role = "Admin",
            };

            var token = await _jwtProvider.GenerateToken(user, true, CancellationToken.None);

            _mockUserRepository.Setup(repo => repo.FindByIdAsync(It.IsAny<Guid>(), CancellationToken.None)).ReturnsAsync(user);

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
                Role = "Admin",
                RefreshToken = "validRefreshToken",
                RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(1)
            };

            var expiredToken = await _jwtProvider.GenerateToken(user, false, CancellationToken.None);
            var tokenDto = new TokenDTO(expiredToken.accessToken, "invalidRefreshToken");

            _mockUserRepository.Setup(repo => repo.FindByIdAsync(It.IsAny<Guid>(), CancellationToken.None)).ReturnsAsync(user);

            // Act 
            Func<Task> act = async () => await _jwtProvider.RefreshToken(tokenDto);

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
                Role = "Admin",
                RefreshToken = "validRefreshToken",
                RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(-1) // expired
            };

            var expiredToken = await _jwtProvider.GenerateToken(user, false, CancellationToken.None);
            var tokenDto = new TokenDTO(expiredToken.accessToken, "validRefreshToken");

            _mockUserRepository.Setup(repo => repo.FindByIdAsync(It.IsAny<Guid>(), CancellationToken.None)).ReturnsAsync(user);

            // Act 
            Func<Task> act = async () => await _jwtProvider.RefreshToken(tokenDto);

            //Assert
            await act.Should().ThrowAsync<RefreshTokenBadRequestException>();
        }
    }
}

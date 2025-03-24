using Microsoft.AspNetCore.Mvc;
using Moq;

namespace AuthorizationService
{
    public class TokenControllerTests
    {
        private readonly Mock<IJwtProvider> _jwtProviderMock;
        private readonly TokenController _controller;

        public TokenControllerTests()
        {
            _jwtProviderMock = new Mock<IJwtProvider>();
            _controller = new TokenController(_jwtProviderMock.Object);
        }

        [Fact]
        public async Task RefreshToken_ReturnsOk_WithUpdatedToken_WhenSuccessful()
        {
            // Arrange
            var tokenDTO = new TokenDTO("access_token", "refresh_token");
            var updatedTokenDTO = new TokenDTO("updated_access_token", "updated_refresh_token");

            _jwtProviderMock.Setup(m => m.RefreshToken(tokenDTO))
                            .ReturnsAsync(updatedTokenDTO);

            // Act
            var result = await _controller.RefreshToken(tokenDTO);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(updatedTokenDTO, okResult.Value);
        }
    }
}

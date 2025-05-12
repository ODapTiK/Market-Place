using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Security.Claims;

namespace UserService
{
    public class AdminControllerTests
    {
        private readonly Mock<IUpdateAdminUseCase> _updateAdminUseCaseMock;
        private readonly Mock<IGetAdminInfoUseCase> _getAdminInfoUseCaseMock;
        private readonly Mock<IUpdateAdminLogoUseCase> _updateAdminLogoUseCaseMock;
        private readonly AdminController _controller;

        public AdminControllerTests()
        {
            _updateAdminUseCaseMock = new Mock<IUpdateAdminUseCase>();
            _getAdminInfoUseCaseMock = new Mock<IGetAdminInfoUseCase>();
            _updateAdminLogoUseCaseMock = new Mock<IUpdateAdminLogoUseCase>();

            var httpContext = new DefaultHttpContext();
            var claims = new[] { new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()) };
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims));
            httpContext.RequestServices = new ServiceCollection()
                .BuildServiceProvider();

            _controller = new AdminController(
                _updateAdminUseCaseMock.Object,
                _getAdminInfoUseCaseMock.Object,
                _updateAdminLogoUseCaseMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext
                }
            };
        }

        [Fact]
        public async Task GetAdmin_ShouldReturnOkResult_WithAdmin()
        {
            // Arrange
            var adminId = _controller.UserId; 
            var admin = new Admin { Id = adminId, Name = "Test Admin" };
            _getAdminInfoUseCaseMock.Setup(u => u.Execute(adminId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(admin);

            // Act
            var result = await _controller.GetAdmin(CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            okResult.Value.Should().BeEquivalentTo(admin);
            _getAdminInfoUseCaseMock.Verify(m => m.Execute(adminId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAdmin_ShouldReturnOkResult()
        {
            // Arrange
            var adminDTO = new AdminDTO { Id = Guid.NewGuid(), Name = "Updated Admin" };

            // Act
            var result = await _controller.UpdateAdmin(adminDTO, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkResult>(result);
            _updateAdminUseCaseMock.Verify(m => m.Execute(adminDTO, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}

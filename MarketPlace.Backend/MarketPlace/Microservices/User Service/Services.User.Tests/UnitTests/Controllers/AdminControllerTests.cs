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
        private readonly Mock<ICreateAdminUseCase> _createAdminUseCaseMock;
        private readonly Mock<IUpdateAdminUseCase> _updateAdminUseCaseMock;
        private readonly Mock<IDeleteAdminUseCase> _deleteAdminUseCaseMock;
        private readonly Mock<IGetAdminInfoUseCase> _getAdminInfoUseCaseMock;
        private readonly AdminController _controller;

        public AdminControllerTests()
        {
            _createAdminUseCaseMock = new Mock<ICreateAdminUseCase>();
            _updateAdminUseCaseMock = new Mock<IUpdateAdminUseCase>();
            _deleteAdminUseCaseMock = new Mock<IDeleteAdminUseCase>();
            _getAdminInfoUseCaseMock = new Mock<IGetAdminInfoUseCase>();

            var httpContext = new DefaultHttpContext();
            var claims = new[] { new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()) };
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims));
            httpContext.RequestServices = new ServiceCollection()
                .BuildServiceProvider();

            _controller = new AdminController(
                _createAdminUseCaseMock.Object,
                _updateAdminUseCaseMock.Object,
                _deleteAdminUseCaseMock.Object,
                _getAdminInfoUseCaseMock.Object)
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
            _getAdminInfoUseCaseMock.Setup(u => u.Execute(adminId))
                .ReturnsAsync(admin);

            // Act
            var result = await _controller.GetAdmin();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            okResult.Value.Should().BeEquivalentTo(admin);
            _getAdminInfoUseCaseMock.Verify(m => m.Execute(adminId), Times.Once);
        }

        [Fact]
        public async Task CreateAdmin_ShouldReturnOkResult_WithNewAdminId()
        {
            // Arrange
            var adminDTO = new AdminDTO { Name = "New Admin" };
            var newAdminId = Guid.NewGuid();
            _createAdminUseCaseMock.Setup(u => u.Execute(adminDTO))
                .ReturnsAsync(newAdminId);

            // Act
            var result = await _controller.CreateAdmin(adminDTO);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            okResult.Value.Should().BeEquivalentTo(newAdminId);
            _createAdminUseCaseMock.Verify(m => m.Execute(adminDTO), Times.Once);
        }

        [Fact]
        public async Task UpdateAdmin_ShouldReturnOkResult()
        {
            // Arrange
            var adminDTO = new AdminDTO { Id = Guid.NewGuid(), Name = "Updated Admin" };

            // Act
            var result = await _controller.UpdateAdmin(adminDTO);

            // Assert
            var okResult = Assert.IsType<OkResult>(result);
            _updateAdminUseCaseMock.Verify(m => m.Execute(adminDTO), Times.Once);
        }

        [Fact]
        public async Task DeleteAdmin_ShouldReturnOkResult()
        {
            // Arrange
            var adminId = _controller.UserId; 

            // Act
            var result = await _controller.DeleteAdmin();

            // Assert
            var okResult = Assert.IsType<OkResult>(result);
            _deleteAdminUseCaseMock.Verify(m => m.Execute(adminId), Times.Once);
        }
    }
}

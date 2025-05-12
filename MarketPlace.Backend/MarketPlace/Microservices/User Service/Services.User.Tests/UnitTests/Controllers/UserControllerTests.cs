using Bogus;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Security.Claims;
using FluentAssertions;

namespace UserService
{
    public class UserControllerTests
    {
        private readonly Mock<IUpdateUserUseCase> _updateUserUseCaseMock;
        private readonly Mock<IGetUserInfoUseCase> _getUserInfoUseCaseMock;
        private readonly Mock<IUpdateUserLogoUseCase> _updateUserLogoUseCaseMock;
        private readonly UserController _controller;
        private readonly Faker _faker;

        public UserControllerTests()
        {
            _updateUserUseCaseMock = new Mock<IUpdateUserUseCase>();
            _getUserInfoUseCaseMock = new Mock<IGetUserInfoUseCase>();
            _updateUserLogoUseCaseMock = new Mock<IUpdateUserLogoUseCase>();

            var httpContext = new DefaultHttpContext();
            var claims = new[] { new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()) };
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims));
            httpContext.RequestServices = new ServiceCollection()
                .BuildServiceProvider();

            _controller = new UserController(
                _updateUserUseCaseMock.Object,
                _getUserInfoUseCaseMock.Object,
                _updateUserLogoUseCaseMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext
                }
            };

            _faker = new Faker();
        }

        [Fact]
        public async Task GetUser_ShouldReturnOkResult_WithUser()
        {
            // Arrange
            var user = new User { Id = Guid.NewGuid(), Name = _faker.Name.FullName() };
            _getUserInfoUseCaseMock.Setup(u => u.Execute(_controller.UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            // Act
            var result = await _controller.GetUser(CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            okResult.Value.Should().BeEquivalentTo(user);
            _getUserInfoUseCaseMock.Verify(m => m.Execute(_controller.UserId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateUser_ShouldReturnOkResult()
        {
            // Arrange
            var userDTO = new UserDTO { Id = Guid.NewGuid(), Name = _faker.Name.FullName() };

            // Act
            var result = await _controller.UpdateUser(userDTO, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkResult>(result);
            _updateUserUseCaseMock.Verify(m => m.Execute(userDTO, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}

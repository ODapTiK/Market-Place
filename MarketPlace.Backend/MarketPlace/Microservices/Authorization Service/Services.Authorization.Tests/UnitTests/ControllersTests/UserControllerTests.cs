using Bogus;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace AuthorizationService
{
    public class UserControllerTests
    {
        private readonly UserController _controller;
        private readonly Mock<IMediator> _mediatorMock;

        public UserControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();

            var httpContext = new DefaultHttpContext();
            httpContext.RequestServices = new ServiceCollection()
                .AddSingleton(_mediatorMock.Object)
                .BuildServiceProvider();

            _controller = new UserController
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext
                }
            };
        }

        [Fact]
        public async Task AuthenticateUser_ReturnsOk_WithToken_WhenSuccessful()
        {
            // Arrange
            var faker = new Faker();

            var authUserDTO = new AuthUserDTO
            {
                Email = faker.Internet.Email(),
                Password = faker.Internet.Password(),
            };

            var token = new TokenDTO("access_token", "refresh_token", "User");
            _mediatorMock.Setup(m => m.Send(It.IsAny<AuthUserRequest>(), default))
                         .ReturnsAsync(token);

            // Act
            var result = await _controller.AuthenticateUser(authUserDTO, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(token, okResult.Value);
        }

        [Fact]
        public async Task CreateUser_ReturnsOk_WithUserId_WhenSuccessful()
        {
            // Arrange
            var faker = new Faker();

            var userDTO = new UserDTO
            {
                Email = faker.Internet.Email(),
                Password = faker.Internet.Password(),
                Role = "User"
            };

            var userId = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.IsAny<CreateUserRequest>(), default))
                         .ReturnsAsync(userId);

            // Act
            var result = await _controller.CreateUser(userDTO, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(userId, okResult.Value);
        }

        [Fact]
        public async Task DeleteUser_ReturnsOk_WhenSuccessful()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteUserRequest>(), default))
                         .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteUser(CancellationToken.None);

            // Assert
            Assert.IsType<OkResult>(result);
        }
    }
}

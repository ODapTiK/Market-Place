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
        private readonly Mock<ICreateUserUseCase> _createUserUseCaseMock;
        private readonly Mock<IUpdateUserUseCase> _updateUserUseCaseMock;
        private readonly Mock<IDeleteUserUseCase> _deleteUserUseCaseMock;
        private readonly Mock<IAddUserOrderUseCase> _addUserOrderUseCaseMock;
        private readonly Mock<IRemoveUserOrderUseCase> _removeUserOrderUseCaseMock;
        private readonly Mock<IGetUserInfoUseCase> _getUserInfoUseCaseMock;
        private readonly UserController _controller;
        private readonly Faker _faker;

        public UserControllerTests()
        {
            _createUserUseCaseMock = new Mock<ICreateUserUseCase>();
            _updateUserUseCaseMock = new Mock<IUpdateUserUseCase>();
            _deleteUserUseCaseMock = new Mock<IDeleteUserUseCase>();
            _addUserOrderUseCaseMock = new Mock<IAddUserOrderUseCase>();
            _removeUserOrderUseCaseMock = new Mock<IRemoveUserOrderUseCase>();
            _getUserInfoUseCaseMock = new Mock<IGetUserInfoUseCase>();

            var httpContext = new DefaultHttpContext();
            var claims = new[] { new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()) };
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims));
            httpContext.RequestServices = new ServiceCollection()
                .BuildServiceProvider();

            _controller = new UserController(
                _createUserUseCaseMock.Object,
                _updateUserUseCaseMock.Object,
                _deleteUserUseCaseMock.Object,
                _addUserOrderUseCaseMock.Object,
                _removeUserOrderUseCaseMock.Object,
                _getUserInfoUseCaseMock.Object)
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
            _getUserInfoUseCaseMock.Setup(u => u.Execute(_controller.UserId))
                .ReturnsAsync(user);

            // Act
            var result = await _controller.GetUser();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            okResult.Value.Should().BeEquivalentTo(user);
            _getUserInfoUseCaseMock.Verify(m => m.Execute(_controller.UserId), Times.Once);
        }

        [Fact]
        public async Task CreateUser_ShouldReturnOkResult_WithNewUserId()
        {
            // Arrange
            var userDTO = new UserDTO { Name = _faker.Name.FullName() };
            var newUserId = Guid.NewGuid();
            _createUserUseCaseMock.Setup(u => u.Execute(userDTO))
                .ReturnsAsync(newUserId);

            // Act
            var result = await _controller.CreateUser(userDTO);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            okResult.Value.Should().BeEquivalentTo(newUserId);
            _createUserUseCaseMock.Verify(m => m.Execute(userDTO), Times.Once);
        }

        [Fact]
        public async Task UpdateUser_ShouldReturnOkResult()
        {
            // Arrange
            var userDTO = new UserDTO { Id = Guid.NewGuid(), Name = _faker.Name.FullName() };

            // Act
            var result = await _controller.UpdateUser(userDTO);

            // Assert
            var okResult = Assert.IsType<OkResult>(result);
            _updateUserUseCaseMock.Verify(m => m.Execute(userDTO), Times.Once);
        }

        [Fact]
        public async Task DeleteUser_ShouldReturnOkResult()
        {
            // Arrange
            var userId = _controller.UserId;

            // Act
            var result = await _controller.DeleteUser();

            // Assert
            var okResult = Assert.IsType<OkResult>(result);
            _deleteUserUseCaseMock.Verify(m => m.Execute(userId), Times.Once);
        }

        [Fact]
        public async Task AddUserOrder_ShouldReturnOkResult()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var userId = _controller.UserId;

            // Act
            var result = await _controller.AddUserOrder(orderId);

            // Assert
            var okResult = Assert.IsType<OkResult>(result);
            _addUserOrderUseCaseMock.Verify(m => m.Execute(userId, orderId), Times.Once);
        }

        [Fact]
        public async Task RemoveUserOrder_ShouldReturnOkResult()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var userId = _controller.UserId;

            // Act
            var result = await _controller.RemoveUserOrder(orderId);

            // Assert
            var okResult = Assert.IsType<OkResult>(result);
            _removeUserOrderUseCaseMock.Verify(m => m.Execute(userId, orderId), Times.Once);
        }
    }
}

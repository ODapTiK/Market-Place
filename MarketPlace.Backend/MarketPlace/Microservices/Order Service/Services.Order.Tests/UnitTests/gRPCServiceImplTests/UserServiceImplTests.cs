using MediatR;
using Moq;
using FluentAssertions;
using Proto.OrderUser;

namespace OrderService
{
    public class UserServiceImplTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly UserServiceImpl _userService;

        public UserServiceImplTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _userService = new UserServiceImpl(_mediatorMock.Object);
        }

        [Fact]
        public async Task DeleteUserOrders_ShouldReturnSuccess_WhenCommandIsHandled()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var request = new DeleteUserOrdersRequest { UserId = userId };
            _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteUserOrdersCommand>(), default))
                .Returns(Task.CompletedTask);

            // Act
            var response = await _userService.DeleteUserOrders(request, TestServerCallContext.Create());

            // Assert
            response.Should().NotBeNull();
            response.Success.Should().BeTrue();
            response.Message.Should().Contain(userId);
        }

        [Fact]
        public async Task DeleteUserOrders_ShouldReturnFailure_WhenExceptionIsThrown()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var request = new DeleteUserOrdersRequest { UserId = userId };
            _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteUserOrdersCommand>(), default))
                .ThrowsAsync(new Exception("Some error occurred"));

            // Act
            var response = await _userService.DeleteUserOrders(request, TestServerCallContext.Create());

            // Assert
            response.Should().NotBeNull();
            response.Success.Should().BeFalse();
            response.Message.Should().Be("Some error occurred");
        }

        [Fact]
        public async Task CreateCart_ShouldReturnSuccess_WhenCommandIsHandled()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var request = new CartRequest { UserId = userId };
            var cartId = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.IsAny<CreateCartCommand>(), default))
                .ReturnsAsync(cartId);

            // Act
            var response = await _userService.CreateCart(request, TestServerCallContext.Create());

            // Assert
            response.Should().NotBeNull();
            response.Success.Should().BeTrue();
            response.Message.Should().Contain(userId);
            response.Message.Should().Contain(cartId.ToString());
        }

        [Fact]
        public async Task CreateCart_ShouldReturnFailure_WhenExceptionIsThrown()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var request = new CartRequest { UserId = userId };
            _mediatorMock.Setup(m => m.Send(It.IsAny<CreateCartCommand>(), default))
                .ThrowsAsync(new Exception("Some error occurred"));

            // Act
            var response = await _userService.CreateCart(request, TestServerCallContext.Create());

            // Assert
            response.Should().NotBeNull();
            response.Success.Should().BeFalse();
            response.Message.Should().Be("Some error occurred");
        }

        [Fact]
        public async Task DeleteCart_ShouldReturnSuccess_WhenCommandIsHandled()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var request = new CartRequest { UserId = userId };
            _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteCartCommand>(), default))
                .Returns(Task.CompletedTask);

            // Act
            var response = await _userService.DeleteCart(request, TestServerCallContext.Create());

            // Assert
            response.Should().NotBeNull();
            response.Success.Should().BeTrue();
            response.Message.Should().Contain(userId);
        }

        [Fact]
        public async Task DeleteCart_ShouldReturnFailure_WhenExceptionIsThrown()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var request = new CartRequest { UserId = userId };
            _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteCartCommand>(), default))
                .ThrowsAsync(new Exception("Some error occurred"));

            // Act
            var response = await _userService.DeleteCart(request, TestServerCallContext.Create());

            // Assert
            response.Should().NotBeNull();
            response.Success.Should().BeFalse();
            response.Message.Should().Be("Some error occurred");
        }
    }
}

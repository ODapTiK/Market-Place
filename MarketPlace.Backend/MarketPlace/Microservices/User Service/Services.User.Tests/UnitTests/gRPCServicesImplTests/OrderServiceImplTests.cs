using Moq;
using FluentAssertions;
using Proto.OrderUser;
using Bogus;
using Grpc.Core;
using Microsoft.AspNetCore.SignalR;

namespace UserService
{
    public class OrderServiceImplTests
    {
        private readonly Mock<IAddUserOrderUseCase> _addUserOrderUseCaseMock;
        private readonly Mock<IRemoveUserOrderUseCase> _removeUserOrderUseCaseMock;
        private readonly Mock<IAddOrderToControlAdminUseCase> _addOrderToControlAdminUseCaseMock;
        private readonly Mock<IRemoveControlAdminOrderUseCase> _removeControlAdminOrderUseCaseMock;
        private readonly Mock<IAddUserNotificationUseCase> _addUserNotificationUseCase;
        private readonly Mock<IHubContext<NotificationHub>> _notificationHub;
        private readonly OrderServiceImpl _orderService;
        private readonly Faker<OrderRequest> _requestFaker;
        public OrderServiceImplTests()
        {
            _addUserOrderUseCaseMock = new Mock<IAddUserOrderUseCase>();
            _removeUserOrderUseCaseMock = new Mock<IRemoveUserOrderUseCase>();
            _addOrderToControlAdminUseCaseMock = new Mock<IAddOrderToControlAdminUseCase>();
            _removeControlAdminOrderUseCaseMock = new Mock<IRemoveControlAdminOrderUseCase>();
            _addUserNotificationUseCase = new Mock<IAddUserNotificationUseCase>();
            _notificationHub = new Mock<IHubContext<NotificationHub>>();
            _requestFaker = new Faker<OrderRequest>()
                .RuleFor(x => x.UserId, Guid.NewGuid().ToString())
                .RuleFor(x => x.OrderId, Guid.NewGuid().ToString());

            _orderService = new OrderServiceImpl(
                _addUserOrderUseCaseMock.Object,
                _removeUserOrderUseCaseMock.Object,
                _addOrderToControlAdminUseCaseMock.Object,
                _removeControlAdminOrderUseCaseMock.Object,
                _addUserNotificationUseCase.Object,
                _notificationHub.Object);
        }

        [Fact]
        public async Task AddUserOrder_ShouldReturnSuccess_WhenOrderIsAdded()
        {
            // Arrange
            var request = _requestFaker.Generate();
            var cancellationToken = new CancellationToken();

            _addUserOrderUseCaseMock.Setup(x => x.Execute(
                It.IsAny<Guid>(), It.IsAny<Guid>(), cancellationToken))
                .Returns(Task.CompletedTask);

            // Act
            var response = await _orderService.AddUserOrder(request, TestServerCallContext.Create());

            // Assert
            response.Should().NotBeNull();
            response.Success.Should().BeTrue();
            response.Message.Should().Contain($"Order \"{request.OrderId}\" added to user \"{request.UserId}\" successfully");
        }

        [Fact]
        public async Task AddUserOrder_ShouldReturnFailure_WhenExceptionIsThrown()
        {
            // Arrange
            var request = _requestFaker.Generate();
            var cancellationToken = new CancellationToken();

            _addUserOrderUseCaseMock.Setup(x => x.Execute(
                It.IsAny<Guid>(), It.IsAny<Guid>(), cancellationToken))
                .ThrowsAsync(new Exception("Failed to add order"));

            // Act
            var response = await _orderService.AddUserOrder(request, TestServerCallContext.Create());

            // Assert
            response.Should().NotBeNull();
            response.Success.Should().BeFalse();
            response.Message.Should().Be("Failed to add order");
        }

        [Fact]
        public async Task RemoveUserOrder_ShouldReturnSuccess_WhenOrderIsRemoved()
        {
            // Arrange
            var request = _requestFaker.Generate();
            var cancellationToken = new CancellationToken();

            _removeUserOrderUseCaseMock.Setup(x => x.Execute(
                It.IsAny<Guid>(), It.IsAny<Guid>(), cancellationToken))
                .Returns(Task.CompletedTask);

            // Act
            var response = await _orderService.RemoveUserOrder(request, TestServerCallContext.Create());

            // Assert
            response.Should().NotBeNull();
            response.Success.Should().BeTrue();
            response.Message.Should().Contain($"Order \"{request.OrderId}\" removed from user \"{request.UserId}\" successfully");
        }

        [Fact]
        public async Task RemoveUserOrder_ShouldReturnFailure_WhenExceptionIsThrown()
        {
            // Arrange
            var request = _requestFaker.Generate();
            var cancellationToken = new CancellationToken();

            _removeUserOrderUseCaseMock.Setup(x => x.Execute(
                It.IsAny<Guid>(), It.IsAny<Guid>(), cancellationToken))
                .ThrowsAsync(new Exception("Failed to remove order"));

            // Act
            var response = await _orderService.RemoveUserOrder(request, TestServerCallContext.Create());

            // Assert
            response.Should().NotBeNull();
            response.Success.Should().BeFalse();
            response.Message.Should().Be("Failed to remove order");
        }

        [Fact]
        public async Task AddOrderToControlAdmin_Success_ReturnsSuccessResponse()
        {
            // Arrange
            var request = new AddOrderToControlAdminRequest
            {
                AdminId = Guid.NewGuid().ToString(),
                OrderId = Guid.NewGuid().ToString()
            };
            var context = new Mock<ServerCallContext>();

            // Act
            var response = await _orderService.AddOrderToControlAdmin(request, context.Object);

            // Assert
            Assert.True(response.Success);
            Assert.Contains($"Order \"{request.OrderId}\" added to admin \"{request.AdminId}\" successfully", response.Message);
            _addOrderToControlAdminUseCaseMock.Verify(useCase => useCase.Execute(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task AddOrderToControlAdmin_Exception_ReturnsFailureResponse()
        {
            // Arrange
            var request = new AddOrderToControlAdminRequest
            {
                AdminId = Guid.NewGuid().ToString(),
                OrderId = Guid.NewGuid().ToString()
            };
            var context = new Mock<ServerCallContext>();
            _addOrderToControlAdminUseCaseMock
                .Setup(useCase => useCase.Execute(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Error adding order"));

            // Act
            var response = await _orderService.AddOrderToControlAdmin(request, context.Object);

            // Assert
            Assert.False(response.Success);
            Assert.Equal("Error adding order", response.Message);
        }

        [Fact]
        public async Task RemoveObsoleteOrderFromUserAndAdmin_Success_ReturnsSuccessResponse()
        {
            // Arrange
            var request = new RemoveObsoleteOrderFromUserAndAdminRequest
            {
                AdminId = Guid.NewGuid().ToString(),
                UserId = Guid.NewGuid().ToString(),
                OrderId = Guid.NewGuid().ToString()
            }
            ;
            var context = new Mock<ServerCallContext>();

            // Act
            var response = await _orderService.RemoveObsoleteOrderFromUserAndAdmin(request, context.Object);

            // Assert
            Assert.True(response.Success);
            Assert.Contains($"Order \"{request.OrderId}\" removed from admin \"{request.AdminId}\" and user \"{request.UserId}\" successfully", response.Message);
            _removeControlAdminOrderUseCaseMock.Verify(useCase => useCase.Execute(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
            _removeUserOrderUseCaseMock.Verify(useCase => useCase.Execute(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task RemoveObsoleteOrderFromUserAndAdmin_Exception_ReturnsFailureResponse()
        {
            // Arrange
            var request = new RemoveObsoleteOrderFromUserAndAdminRequest
            {
                AdminId = Guid.NewGuid().ToString(),
                UserId = Guid.NewGuid().ToString(),
                OrderId = Guid.NewGuid().ToString()
            }
            ;
            var context = new Mock<ServerCallContext>();
            _removeControlAdminOrderUseCaseMock
                .Setup(useCase => useCase.Execute(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Error removing order"));

            // Act
            var response = await _orderService.RemoveObsoleteOrderFromUserAndAdmin(request, context.Object);

            // Assert
            Assert.False(response.Success);
            Assert.Equal("Error removing order", response.Message);
        }
    }
}

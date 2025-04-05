using Moq;
using FluentAssertions;
using Proto.OrderUser;
using Bogus;

namespace UserService
{
    public class OrderServiceImplTests
    {
        private readonly Mock<IAddUserOrderUseCase> _addUserOrderUseCaseMock;
        private readonly Mock<IRemoveUserOrderUseCase> _removeUserOrderUseCaseMock;
        private readonly OrderServiceImpl _orderService;
        private readonly Faker<OrderRequest> _requestFaker;
        public OrderServiceImplTests()
        {
            _addUserOrderUseCaseMock = new Mock<IAddUserOrderUseCase>();
            _removeUserOrderUseCaseMock = new Mock<IRemoveUserOrderUseCase>();
            _requestFaker = new Faker<OrderRequest>()
                .RuleFor(x => x.UserId, Guid.NewGuid().ToString())
                .RuleFor(x => x.OrderId, Guid.NewGuid().ToString());

            _orderService = new OrderServiceImpl(
                _addUserOrderUseCaseMock.Object,
                _removeUserOrderUseCaseMock.Object);
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
    }
}

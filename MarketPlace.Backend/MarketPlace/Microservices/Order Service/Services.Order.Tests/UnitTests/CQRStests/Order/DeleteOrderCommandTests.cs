using Bogus;
using FluentAssertions;
using Moq;
using FluentValidation.TestHelper;
using Proto.OrderUser;
using Hangfire;

namespace OrderService
{
    public class DeleteOrderCommandTests
    {
        private readonly Mock<IOrderRepository> _orderRepositoryMock;
        private readonly Mock<IObsoleteOrderCollector> _obsoleteOrderCollectorMock;
        private readonly Mock<IBackgroundJobClient> _backgroundJobClientMock;
        private readonly DeleteOrderCommandHandler _handler;
        private readonly Faker _faker;
        private readonly DeleteOrderCommandValidator _validator;
        private readonly Mock<OrderUserService.OrderUserServiceClient> _orderUserServiceClientMock; 
        public DeleteOrderCommandTests()
        {
            _orderRepositoryMock = new Mock<IOrderRepository>();
            _orderUserServiceClientMock = new Mock<OrderUserService.OrderUserServiceClient>();
            _obsoleteOrderCollectorMock = new Mock<IObsoleteOrderCollector>();
            _backgroundJobClientMock = new Mock<IBackgroundJobClient>();
            _handler = new DeleteOrderCommandHandler(_orderRepositoryMock.Object, 
                                                     _orderUserServiceClientMock.Object, 
                                                     _obsoleteOrderCollectorMock.Object,
                                                     _backgroundJobClientMock.Object);
            _faker = new Faker();
            _validator = new DeleteOrderCommandValidator();
        }

        [Fact]
        public async Task Handle_ShouldDeleteOrder_WhenOrderExists()
        {
            // Arrange
            var orderId = _faker.Random.Guid();
            var command = new DeleteOrderCommand { Id = orderId };
            var existingOrder = new Order { Id = orderId }; 

            _orderRepositoryMock.Setup(repo => repo.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingOrder);

            var mockCall = CallHelpers.CreateAsyncUnaryCall(new Response
            {
                Message = "Test",
                Success = true
            });
            _orderUserServiceClientMock
                .Setup(m => m.RemoveUserOrderAsync(
                    It.IsAny<OrderRequest>(), null, null, CancellationToken.None))
                .Returns(mockCall);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _orderRepositoryMock.Verify(repo => repo.DeleteAsync(existingOrder, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowEntityNotFoundException_WhenOrderDoesNotExist()
        {
            // Arrange
            var orderId = _faker.Random.Guid();
            var command = new DeleteOrderCommand { Id = orderId };

            _orderRepositoryMock.Setup(repo => repo.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Order?)null); 

            // Act
            var act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<EntityNotFoundException>();
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenCommandIsInvalid()
        {
            // Arrange
            var command = new DeleteOrderCommand
            {
                Id = Guid.Empty
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Id);
        }

        [Fact]

        public void Handle_ShouldThrowGRPCRequestFailException_WhenGRPCRequestReturnsFailure()
        {
            // Arrange
            var orderId = _faker.Random.Guid();
            var command = new DeleteOrderCommand { Id = orderId };
            var existingOrder = new Order { Id = orderId };

            _orderRepositoryMock.Setup(repo => repo.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingOrder);

            var mockCall = CallHelpers.CreateAsyncUnaryCall(new Response
            {
                Message = "Failure test",
                Success = false
            });
            _orderUserServiceClientMock
                .Setup(m => m.RemoveUserOrderAsync(
                    It.IsAny<OrderRequest>(), null, null, CancellationToken.None))
                .Returns(mockCall);

            // Act
            var act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            act.Should().ThrowAsync<GRPCRequestFailException>().WithMessage("Failure test");
        }
    }
}

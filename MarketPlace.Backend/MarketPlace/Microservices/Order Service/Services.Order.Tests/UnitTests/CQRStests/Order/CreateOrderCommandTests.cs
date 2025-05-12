using Bogus;
using FluentValidation.TestHelper;
using FluentAssertions;
using Moq;
using Proto.OrderUser;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using Proto.OrderProduct;

namespace OrderService
{
    public class CreateOrderCommandTests 
    {
        private readonly Mock<IOrderRepository> _orderRepositoryMock;
        private readonly CreateOrderCommandHandler _handler;
        private readonly Faker<OrderPointDTO> _orderPointFaker;
        private readonly CreateOrderCommandValidator _validator;
        private readonly Mock<OrderUserService.OrderUserServiceClient> _orderUserServiceClientMock;
        private readonly Mock<OrderProductService.OrderProductServiceClient> _orderProductServiceClientMock;
        private readonly Mock<IObsoleteOrderCollector> _ordersCollectorMock;    
        private readonly Mock<IBackgroundJobClient> _backgroundJobClientMock;

        public CreateOrderCommandTests()
        {
            _orderRepositoryMock = new Mock<IOrderRepository>();
            _orderUserServiceClientMock = new Mock<OrderUserService.OrderUserServiceClient>();
            _orderProductServiceClientMock = new Mock<OrderProductService.OrderProductServiceClient>();
            _ordersCollectorMock = new Mock<IObsoleteOrderCollector>();
            _backgroundJobClientMock = new Mock<IBackgroundJobClient>();

            _handler = new CreateOrderCommandHandler(_orderRepositoryMock.Object, 
                                                     _orderUserServiceClientMock.Object, 
                                                     _orderProductServiceClientMock.Object,
                                                     _ordersCollectorMock.Object,
                                                     _backgroundJobClientMock.Object);

            _validator = new CreateOrderCommandValidator();

            _orderPointFaker = new Faker<OrderPointDTO>()
                .RuleFor(op => op.ProductId, f => Guid.NewGuid())
                .RuleFor(op => op.NumberOfUnits, f => f.Random.Int(1, 10));
        }

        [Fact]
        public async Task Handle_ShouldCreateOrder_WhenCommandIsValid()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var orderPoints = _orderPointFaker.Generate(5); 
            var command = new CreateOrderCommand
            {
                UserId = userId,
                Points = orderPoints,
            };

            _orderRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Guid.NewGuid());
            _ordersCollectorMock.Setup(repo => repo.RemoveObsoleteOrderAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var mockCall = CallHelpers.CreateAsyncUnaryCall(new Proto.OrderUser.Response
            {
                Message = "Test",
                Success = true
            });
            _orderUserServiceClientMock
                .Setup(m => m.AddUserOrderAsync(
                    It.IsAny<Proto.OrderUser.OrderRequest>(), null, null, CancellationToken.None))
                .Returns(mockCall);

            var orderProductServiceMockCall = CallHelpers.CreateAsyncUnaryCall(new OrderResponse
            {
                Message = "Test",
                Success = true,
                TotalPrice = 15
            });
            _orderProductServiceClientMock
                .Setup(m => m.CalculateTotalPriceAsync(
                    It.IsAny<Proto.OrderProduct.OrderRequest>(), null, null, CancellationToken.None))
                .Returns(orderProductServiceMockCall);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            _backgroundJobClientMock.Verify(x => x.Create(
                It.Is<Job>(job => job.Method.Name == nameof(IObsoleteOrderCollector.RemoveObsoleteOrderAsync)),
                It.IsAny<IState>()));

            result.Should().NotBe(Guid.Empty);

            _orderRepositoryMock.Verify(repo => repo.CreateAsync(It.Is<Order>(o =>
                o.UserId == userId &&
                o.OrderPoints.Count == orderPoints.Count),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenCommandIsInvalid()
        {
            // Arrange
            var command = new CreateOrderCommand
            {
                UserId = Guid.Empty,
                Points = new List<OrderPointDTO> { new OrderPointDTO { ProductId = Guid.Empty, NumberOfUnits = 1 } } 
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.UserId);
            result.ShouldHaveValidationErrorFor(x => x.Points);
        }

        [Fact]
        public void Handle_ShouldThrowGRPCRequestFailException_WhenAddUserOrderGRPCRequestReturnsFailure()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var orderPoints = _orderPointFaker.Generate(5);
            var command = new CreateOrderCommand
            {
                UserId = userId,
                Points = orderPoints
            };

            _orderRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Guid.NewGuid());

            var mockCall = CallHelpers.CreateAsyncUnaryCall(new Proto.OrderUser.Response
            {
                Message = "Failure test",
                Success = false
            });
            _orderUserServiceClientMock
                .Setup(m => m.AddUserOrderAsync(
                    It.IsAny<Proto.OrderUser.OrderRequest>(), null, null, CancellationToken.None))
                .Returns(mockCall);

            var orderProductServiceMockCall = CallHelpers.CreateAsyncUnaryCall(new OrderResponse
            {
                Message = "Test",
                Success = true,
                TotalPrice = 15
            });
            _orderProductServiceClientMock
                .Setup(m => m.CalculateTotalPriceAsync(
                    It.IsAny<Proto.OrderProduct.OrderRequest>(), null, null, CancellationToken.None))
                .Returns(orderProductServiceMockCall);

            // Act
            var act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            act.Should().ThrowAsync<GRPCRequestFailException>().WithMessage("Failure test");
        }

        [Fact]
        public void Handle_ShouldThrowGRPCRequestFailException_WhenCalculateTotalPriceGRPCRequestReturnsFailure()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var orderPoints = _orderPointFaker.Generate(5);
            var command = new CreateOrderCommand
            {
                UserId = userId,
                Points = orderPoints
            };

            _orderRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Guid.NewGuid());

            var mockCall = CallHelpers.CreateAsyncUnaryCall(new Proto.OrderUser.Response
            {
                Message = "Test",
                Success = false
            });
            _orderUserServiceClientMock
                .Setup(m => m.AddUserOrderAsync(
                    It.IsAny<Proto.OrderUser.OrderRequest>(), null, null, CancellationToken.None))
                .Returns(mockCall);

            var orderProductServiceMockCall = CallHelpers.CreateAsyncUnaryCall(new OrderResponse
            {
                Message = "Faulure test",
                Success = true,
                TotalPrice = 15
            });
            _orderProductServiceClientMock
                .Setup(m => m.CalculateTotalPriceAsync(
                    It.IsAny<Proto.OrderProduct.OrderRequest>(), null, null, CancellationToken.None))
                .Returns(orderProductServiceMockCall);

            // Act
            var act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            act.Should().ThrowAsync<GRPCRequestFailException>().WithMessage("Failure test");
        }
    }
}

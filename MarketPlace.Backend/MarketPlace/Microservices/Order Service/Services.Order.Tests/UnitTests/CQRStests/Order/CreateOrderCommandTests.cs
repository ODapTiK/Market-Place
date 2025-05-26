using Bogus;
using FluentAssertions;
using FluentValidation.TestHelper;
using Moq;
using Proto.OrderProduct;
using Proto.OrderUser;

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

        public CreateOrderCommandTests()
        {
            _orderRepositoryMock = new Mock<IOrderRepository>();
            _orderUserServiceClientMock = new Mock<OrderUserService.OrderUserServiceClient>();
            _orderProductServiceClientMock = new Mock<OrderProductService.OrderProductServiceClient>();

            _handler = new CreateOrderCommandHandler(_orderRepositoryMock.Object, 
                                                     _orderUserServiceClientMock.Object, 
                                                     _orderProductServiceClientMock.Object);

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
            var controlAdminId = Guid.NewGuid();
            var orderPoints = _orderPointFaker.Generate(5); 
            var command = new CreateOrderCommand
            {
                UserId = userId,
                ControlAdminId = controlAdminId,
                Points = orderPoints,
            };

            _orderRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Guid.NewGuid());

            var mockCall = CallHelpers.CreateAsyncUnaryCall(new Proto.OrderUser.Response
            {
                Message = "Test",
                Success = true
            });
            _orderUserServiceClientMock
                .Setup(m => m.AddUserOrderAsync(
                    It.IsAny<Proto.OrderUser.OrderRequest>(), null, null, CancellationToken.None))
                .Returns(mockCall);
            _orderUserServiceClientMock
                .Setup(m => m.AddOrderToControlAdminAsync(
                    It.IsAny<AddOrderToControlAdminRequest>(), null, null, CancellationToken.None))
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

            var productsInfo = CallHelpers.CreateAsyncUnaryCall(new ProductInfoResponse
            {
                Message = "Test",
                Success = true,
                ProductCategory = "Test category",
                ProductDescription = "Test description",
                ProductImage = "base64 image",
                ProductName = "Test name",
                ProductType = "Test type"
            });
            _orderProductServiceClientMock
                .Setup(m => m.GetProductInfoAsync(
                    It.IsAny<GetProductInfoRequest>(), null, null, CancellationToken.None))
                .Returns(productsInfo);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

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

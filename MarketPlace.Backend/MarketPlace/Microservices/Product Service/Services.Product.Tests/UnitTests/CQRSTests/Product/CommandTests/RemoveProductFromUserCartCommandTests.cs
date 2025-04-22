using Bogus;
using FluentAssertions;
using Moq;
using Proto.OrderProduct;

namespace ProductService
{
    public class RemoveProductFromUserCartCommandTests
    {
        private readonly Mock<OrderProductService.OrderProductServiceClient> _orderServiceClientMock;
        private readonly Mock<IProductRepository> _productRepositoryMock;
        private readonly RemoveProductFromUserCartCommandHandler _handler;
        private readonly Faker _faker;

        public RemoveProductFromUserCartCommandTests()
        {
            _orderServiceClientMock = new Mock<OrderProductService.OrderProductServiceClient>();
            _productRepositoryMock = new Mock<IProductRepository>();
            _handler = new RemoveProductFromUserCartCommandHandler(
                _orderServiceClientMock.Object,
                _productRepositoryMock.Object);
            _faker = new Faker();
        }

        [Fact]
        public async Task Handle_ShouldCallOrderService_WhenProductExists()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var command = new RemoveProductFromUserCartCommand()
            {
                ProductId = productId,
                UserId = userId
            };
            var product = new Product { Id = productId };

            _productRepositoryMock.Setup(x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            var mockCall = CallHelpers.CreateAsyncUnaryCall(new Response
            {
                Message = "Test",
                Success = true
            });
            _orderServiceClientMock.Setup(x => x.RemoveProductFromCartAsync(
                It.IsAny<ProductRequest>(), null, null, It.IsAny<CancellationToken>()))
                .Returns(mockCall);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _productRepositoryMock.Verify(x =>
                x.GetByIdAsync(productId, It.IsAny<CancellationToken>()),
                Times.Once);

            _orderServiceClientMock.Verify(x => x.RemoveProductFromCartAsync(
                It.Is<ProductRequest>(r =>
                    r.ProductId == productId.ToString() &&
                    r.UserId == userId.ToString()),
                null, null, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowEntityNotFoundException_WhenProductNotExists()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var command = new RemoveProductFromUserCartCommand()
            {
                ProductId = productId,
                UserId = userId
            };

            _productRepositoryMock.Setup(x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Product?)null);

            // Act & Assert
            await _handler.Invoking(x => x.Handle(command, CancellationToken.None))
                .Should().ThrowAsync<EntityNotFoundException>();
        }

        [Fact]
        public async Task Handle_ShouldThrowGRPCRequestFailException_WhenOrderServiceFails()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var command = new RemoveProductFromUserCartCommand()
            {
                ProductId = productId,
                UserId = userId
            };
            var product = new Product { Id = productId };
            var errorMessage = "Failed to remove product from cart";

            _productRepositoryMock.Setup(x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            var mockCall = CallHelpers.CreateAsyncUnaryCall(new Response
            {
                Message = errorMessage,
                Success = false
            });
            _orderServiceClientMock.Setup(x => x.RemoveProductFromCartAsync(
                It.IsAny<ProductRequest>(), null, null, It.IsAny<CancellationToken>()))
                .Returns(mockCall);

            // Act & Assert
            await _handler.Invoking(x => x.Handle(command, CancellationToken.None))
                .Should().ThrowAsync<GRPCRequestFailException>()
                .WithMessage(errorMessage);
        }
    }
}

using Bogus;
using Grpc.Core;
using MediatR;
using Moq;
using Proto.OrderProduct;
using FluentAssertions;

namespace OrderService
{
    public class ProductServiceImplTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly ProductServiceImpl _productService;
        private readonly Faker _faker;

        public ProductServiceImplTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _productService = new ProductServiceImpl(_mediatorMock.Object);
            _faker = new Faker();
        }

        [Fact]
        public async Task AddProductToCart_ShouldReturnSuccess_WhenEverythingWorks()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var productId = Guid.NewGuid().ToString();
            var cartId = Guid.NewGuid();

            var request = new ProductRequest
            {
                UserId = userId,
                ProductId = productId
            };

            _mediatorMock.Setup(m => m.Send(It.Is<GetUserCartQuery>(q => q.UserId == Guid.Parse(userId)), default))
                .ReturnsAsync(new Cart { Id = cartId });

            _mediatorMock.Setup(m => m.Send(It.Is<AddOrderPointCommand>(c =>
                c.ProductId == Guid.Parse(productId) && c.CartId == cartId), default))
                .Returns(Task.CompletedTask);

            // Act
            var response = await _productService.AddProductToCart(request, TestServerCallContext.Create());

            // Assert
            response.Should().NotBeNull();
            response.Success.Should().BeTrue();
            response.Message.Should().Contain(productId);
            response.Message.Should().Contain(cartId.ToString());
        }

        [Fact]
        public async Task AddProductToCart_ShouldReturnFailure_WhenMediatorThrows()
        {
            // Arrange
            var request = new ProductRequest
            {
                UserId = Guid.NewGuid().ToString(),
                ProductId = Guid.NewGuid().ToString()
            };

            var expectedException = new Exception("Test error");

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetUserCartQuery>(), default))
                .ThrowsAsync(expectedException);

            // Act
            var response = await _productService.AddProductToCart(request, TestServerCallContext.Create());

            // Assert
            response.Should().NotBeNull();
            response.Success.Should().BeFalse();
            response.Message.Should().Be(expectedException.Message);
        }

        [Fact]
        public async Task RemoveProductFromCart_ShouldReturnSuccess_WhenEverythingWorks()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var productId = Guid.NewGuid().ToString();
            var cartId = Guid.NewGuid();

            var request = new ProductRequest
            {
                UserId = userId,
                ProductId = productId
            };

            _mediatorMock.Setup(m => m.Send(It.Is<GetUserCartQuery>(q => q.UserId == Guid.Parse(userId)), default))
                .ReturnsAsync(new Cart { Id = cartId });

            _mediatorMock.Setup(m => m.Send(It.Is<RemoveOrderPointCommand>(c =>
                c.ProductId == Guid.Parse(productId) && c.CartId == cartId), default))
                .Returns(Task.CompletedTask);

            // Act
            var response = await _productService.RemoveProductFromCart(request, TestServerCallContext.Create());

            // Assert
            response.Should().NotBeNull();
            response.Success.Should().BeTrue();
            response.Message.Should().Contain(productId);
            response.Message.Should().Contain(cartId.ToString());
        }

        [Fact]
        public async Task RemoveProductFromCart_ShouldReturnFailure_WhenMediatorThrows()
        {
            // Arrange
            var request = new ProductRequest
            {
                UserId = Guid.NewGuid().ToString(),
                ProductId = Guid.NewGuid().ToString()
            };

            var expectedException = new Exception("Test error");

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetUserCartQuery>(), default))
                .ThrowsAsync(expectedException);

            // Act
            var response = await _productService.RemoveProductFromCart(request, TestServerCallContext.Create());

            // Assert
            response.Should().NotBeNull();
            response.Success.Should().BeFalse();
            response.Message.Should().Be(expectedException.Message);
        }

        [Fact]
        public async Task DeleteProductFromAllCarts_ShouldReturnSuccess_WhenEverythingWorks()
        {
            // Arrange
            var productId = Guid.NewGuid().ToString();
            var request = new DeleteProductRequest { ProductId = productId };

            _mediatorMock.Setup(m => m.Send(It.Is<RemoveProductFromCartsCommand>(c =>
                c.ProductId == Guid.Parse(productId)), default))
                .Returns(Task.CompletedTask);

            // Act
            var response = await _productService.DeleteProductFromAllCarts(request, TestServerCallContext.Create());

            // Assert
            response.Should().NotBeNull();
            response.Success.Should().BeTrue();
            response.Message.Should().Contain(productId);
        }

        [Fact]
        public async Task DeleteProductFromAllCarts_ShouldReturnFailure_WhenMediatorThrows()
        {
            // Arrange
            var request = new DeleteProductRequest { ProductId = Guid.NewGuid().ToString() };

            var expectedException = new Exception("Test error");

            _mediatorMock.Setup(m => m.Send(It.IsAny<RemoveProductFromCartsCommand>(), default))
                .ThrowsAsync(expectedException);

            // Act
            var response = await _productService.DeleteProductFromAllCarts(request, TestServerCallContext.Create());

            // Assert
            response.Should().NotBeNull();
            response.Success.Should().BeFalse();
            response.Message.Should().Be(expectedException.Message);
        }

        [Fact]
        public async Task AddProductToCart_ShouldReturnFailure_WhenInvalidUserIdFormat()
        {
            // Arrange
            var request = new ProductRequest
            {
                UserId = "invalid-guid",
                ProductId = Guid.NewGuid().ToString()
            };

            // Act
            var response = await _productService.AddProductToCart(request, TestServerCallContext.Create());

            // Assert
            response.Should().NotBeNull();
            response.Success.Should().BeFalse();
        }

        [Fact]
        public async Task RemoveProductFromCart_ShouldReturnFailure_WhenInvalidProductIdFormat()
        {
            // Arrange
            var request = new ProductRequest
            {
                UserId = Guid.NewGuid().ToString(),
                ProductId = "invalid-guid"
            };

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetUserCartQuery>(), default))
                .ReturnsAsync(new Cart { Id = Guid.NewGuid() });

            // Act
            var response = await _productService.RemoveProductFromCart(request, TestServerCallContext.Create());

            // Assert
            response.Should().NotBeNull();
            response.Success.Should().BeFalse();
        }
    }
}

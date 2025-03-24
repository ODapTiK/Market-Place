using Bogus;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace OrderService
{
    public class CartControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CartController _controller;
        private readonly Faker<Cart> _faker;    

        public CartControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();

            _faker = new Faker<Cart>()
                .RuleFor(c => c.Id, f => f.Random.Guid())
                .RuleFor(c => c.UserId, f => f.Random.Guid())
                .RuleFor(c => c.Products, f => f.Make(f.Random.Int(1, 10), () => f.Random.Guid()));

            var httpContext = new DefaultHttpContext();
            httpContext.RequestServices = new ServiceCollection()
                .AddSingleton(_mediatorMock.Object)
                .BuildServiceProvider();

            _controller = new CartController
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext
                }
            };
        }

        [Fact]
        public async Task CreateCart_ShouldReturnOk_WithCartId()
        {
            // Arrange
            var expectedCartId = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.IsAny<CreateCartCommand>(), default))
                .ReturnsAsync(expectedCartId);

            // Act
            var result = await _controller.CreateCart();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(expectedCartId, okResult.Value);
            _mediatorMock.Verify(m => m.Send(It.IsAny<CreateCartCommand>(), default), Times.Once);
        }

        [Fact]
        public async Task DeleteCart_ShouldReturnOk()
        {
            // Arrange
            var cartId = Guid.NewGuid();

            // Act
            var result = await _controller.DeleteCart(cartId);

            // Assert
            Assert.IsType<OkResult>(result);
            _mediatorMock.Verify(m => m.Send(It.IsAny<DeleteCartCommand>(), default), Times.Once);
        }

        [Fact]
        public async Task AddOrderPoint_ShouldReturnOk()
        {
            // Arrange
            var cartId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            // Act
            var result = await _controller.AddOrderPoint(cartId, productId);

            // Assert
            Assert.IsType<OkResult>(result);
            _mediatorMock.Verify(m => m.Send(It.IsAny<AddOrderPointCommand>(), default), Times.Once);
        }

        [Fact]
        public async Task RemoveOrderPoint_ShouldReturnOk()
        {
            // Arrange
            var cartId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            // Act
            var result = await _controller.RemoveOrderPoint(cartId, productId);

            // Assert
            Assert.IsType<OkResult>(result);
            _mediatorMock.Verify(m => m.Send(It.IsAny<RemoveOrderPointCommand>(), default), Times.Once);
        }

        [Fact]
        public async Task GetUserCart_ShouldReturnOk_WithUserCart()
        {
            // Arrange
            var expectedCart = _faker.Generate();

            _mediatorMock.Setup(m => m.Send(It.IsAny < GetUserCartQuery > (), default))
                .ReturnsAsync(expectedCart);

            // Act
            var result = await _controller.GetUserCart();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            okResult.Value.Should().BeEquivalentTo(expectedCart);
            _mediatorMock.Verify(m => m.Send(It.IsAny < GetUserCartQuery > (), default), Times.Once);
        }

        [Fact]
        public async Task GetCart_ShouldReturnOk_WithCart()
        {
            // Arrange
            var cartId = Guid.NewGuid();
            var expectedCart = _faker.Generate();

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetCartByIdQuery>(), default))
                .ReturnsAsync(expectedCart);

            // Act
            var result = await _controller.GetCart(cartId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            okResult.Value.Should().BeEquivalentTo(expectedCart);
            _mediatorMock.Verify(m => m.Send(It.IsAny<GetCartByIdQuery>(), default), Times.Once);
        }
    }
}

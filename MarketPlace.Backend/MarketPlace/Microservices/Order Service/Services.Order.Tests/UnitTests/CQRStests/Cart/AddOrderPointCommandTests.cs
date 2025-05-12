using Bogus;
using FluentAssertions;
using Moq;
using FluentValidation.TestHelper;

namespace OrderService
{
    public class AddOrderPointCommandTests
    {
        private readonly Mock<ICartRepository> _cartRepositoryMock;
        private readonly AddOrderPointCommandHandler _handler;
        private readonly Faker _faker;
        private readonly AddOrderPointCommandValidator _validator;

        public AddOrderPointCommandTests()
        {
            _cartRepositoryMock = new Mock<ICartRepository>();
            _handler = new AddOrderPointCommandHandler(_cartRepositoryMock.Object);
            _faker = new Faker();
            _validator = new AddOrderPointCommandValidator();
        }

        [Fact]
        public async Task Handle_ShouldAddOrderPoint_WhenCartExistsAndProductNotInCart()
        {
            // Arrange
            var cartId = _faker.Random.Guid();
            var productId = _faker.Random.Guid();
            var command = new AddOrderPointCommand { CartId = cartId, ProductId = productId };
            var existingCart = new Cart { Id = cartId, Products = new List<Guid>() };

            _cartRepositoryMock.Setup(repo => repo.GetByIdAsync(cartId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingCart);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _cartRepositoryMock.Verify(repo =>
                repo.AddOrderPointAsync(existingCart, productId, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowEntityNotFoundException_WhenCartDoesNotExist()
        {
            // Arrange
            var cartId = _faker.Random.Guid();
            var productId = _faker.Random.Guid();
            var command = new AddOrderPointCommand { CartId = cartId, ProductId = productId };

            _cartRepositoryMock.Setup(repo => repo.GetByIdAsync(cartId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Cart?)null);

            // Act & Assert
            await _handler.Invoking(x => x.Handle(command, CancellationToken.None))
                .Should().ThrowAsync<EntityNotFoundException>();
        }

        [Fact]
        public async Task Handle_ShouldThrowEntityAlreadyExistsException_WhenProductAlreadyInCart()
        {
            // Arrange
            var cartId = _faker.Random.Guid();
            var productId = _faker.Random.Guid();
            var command = new AddOrderPointCommand { CartId = cartId, ProductId = productId };
            var existingCart = new Cart
            {
                Id = cartId,
                Products = new List<Guid> { productId }
            };

            _cartRepositoryMock.Setup(repo => repo.GetByIdAsync(cartId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingCart);

            // Act & Assert
            await _handler.Invoking(x => x.Handle(command, CancellationToken.None))
                .Should().ThrowAsync<EntityAlreadyExistsException>();
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenCommandIsInvalid()
        {
            // Arrange
            var command = new AddOrderPointCommand
            {
                ProductId = Guid.Empty,
                CartId = Guid.Empty
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.ProductId);
            result.ShouldHaveValidationErrorFor(x => x.CartId);
        }

        [Fact]
        public void Validate_ShouldPass_WhenCommandIsValid()
        {
            // Arrange
            var command = new AddOrderPointCommand
            {
                ProductId = _faker.Random.Guid(),
                CartId = _faker.Random.Guid()
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.ProductId);
            result.ShouldNotHaveValidationErrorFor(x => x.CartId);
        }
    }
}

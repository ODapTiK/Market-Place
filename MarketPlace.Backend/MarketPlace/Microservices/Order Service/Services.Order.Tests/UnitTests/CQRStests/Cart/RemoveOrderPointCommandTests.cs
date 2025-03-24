using Bogus;
using FluentAssertions;
using Moq;
using FluentValidation.TestHelper;

namespace OrderService
{
    public class RemoveOrderPointCommandTests
    {
        private readonly Mock<ICartRepository> _cartRepositoryMock;
        private readonly RemoveOrderPointCommandHandler _handler;
        private readonly Faker _faker;
        private readonly RemoveOrderPointCommandValidator _validator;

        public RemoveOrderPointCommandTests()
        {
            _cartRepositoryMock = new Mock<ICartRepository>();
            _handler = new RemoveOrderPointCommandHandler(_cartRepositoryMock.Object);
            _faker = new Faker();
            _validator = new RemoveOrderPointCommandValidator();
        }

        [Fact]
        public async Task Handle_ShouldRemoveOrderPoint_WhenCartExists()
        {
            // Arrange
            var cartId = _faker.Random.Guid();
            var productId = _faker.Random.Guid();
            var command = new RemoveOrderPointCommand 
            { 
                CartId = cartId, 
                ProductId = productId 
            };
            var existingCart = new Cart { Id = cartId };

            _cartRepositoryMock.Setup(repo => repo.GetByIdAsync(cartId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingCart); 

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _cartRepositoryMock.Verify(repo => repo.GetByIdAsync(cartId, It.IsAny<CancellationToken>()), Times.Once);
            _cartRepositoryMock.Verify(repo => repo.RemoveOrderPointAsync(existingCart, productId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowEntityNotFoundException_WhenCartDoesNotExist()
        {
            // Arrange
            var cartId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var command = new RemoveOrderPointCommand { CartId = cartId, ProductId = productId };

            _cartRepositoryMock.Setup(repo => repo.GetByIdAsync(cartId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Cart?)null); 

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<EntityNotFoundException>();
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenCommandIsInvalid()
        {
            // Arrange
            var command = new RemoveOrderPointCommand()
            {
                CartId = Guid.Empty,
                ProductId = Guid.Empty,
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.CartId);
            result.ShouldHaveValidationErrorFor(x => x.ProductId);
        }
    }
}

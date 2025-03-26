using Bogus;
using FluentAssertions;
using Moq;
using FluentValidation.TestHelper;

namespace OrderService
{
    public class DeleteCartCommandTests
    {
        private readonly Mock<ICartRepository> _cartRepositoryMock;
        private readonly DeleteCartCommandHandler _handler;
        private readonly Faker _faker;
        private readonly DeleteCartCommandValidator _validator;

        public DeleteCartCommandTests()
        {
            _cartRepositoryMock = new Mock<ICartRepository>();
            _handler = new DeleteCartCommandHandler(_cartRepositoryMock.Object);
            _faker = new Faker();
            _validator = new DeleteCartCommandValidator();
        }

        [Fact]
        public async Task Handle_ShouldDeleteCart_WhenCartExists()
        {
            // Arrange
            var cartId = _faker.Random.Guid();
            var command = new DeleteCartCommand { Id = cartId };
            var existingCart = new Cart { Id = cartId };

            _cartRepositoryMock.Setup(repo => repo.GetByIdAsync(cartId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingCart); 

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _cartRepositoryMock.Verify(repo => repo.GetByIdAsync(cartId, It.IsAny<CancellationToken>()), Times.Once);
            _cartRepositoryMock.Verify(repo => repo.DeleteAsync(existingCart, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowEntityNotFoundException_WhenCartDoesNotExist()
        {
            // Arrange
            var cartId = _faker.Random.Guid();
            var command = new DeleteCartCommand { Id = cartId };

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
            var command = new DeleteCartCommand()
            {
                Id = Guid.Empty
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Id);
        }
    }
}

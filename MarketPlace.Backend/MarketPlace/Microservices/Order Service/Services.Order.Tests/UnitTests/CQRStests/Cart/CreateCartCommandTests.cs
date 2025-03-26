using Bogus;
using FluentAssertions;
using Moq;
using FluentValidation.TestHelper;

namespace OrderService
{
    public class CreateCartCommandTests
    {
        private readonly Mock<ICartRepository> _cartRepositoryMock;
        private readonly CreateCartCommandHandler _handler;
        private readonly Faker _faker;
        private readonly CreateCartCommandValidator _validator;

        public CreateCartCommandTests()
        {
            _cartRepositoryMock = new Mock<ICartRepository>();
            _handler = new CreateCartCommandHandler(_cartRepositoryMock.Object);
            _faker = new Faker();
            _validator = new CreateCartCommandValidator();
        }

        [Fact]
        public async Task Handle_ShouldCreateCartAndReturnItsId_WhenNoExistingCart()
        {
            // Arrange
            var userId = _faker.Random.Guid();
            var command = new CreateCartCommand { UserId = userId };
            var cartId = Guid.NewGuid();

            _cartRepositoryMock.Setup(repo => repo.GetUserCartAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Cart?)null); 

            _cartRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<Cart>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(cartId);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().Be(cartId);
            _cartRepositoryMock.Verify(repo => repo.GetUserCartAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
            _cartRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<Cart>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowEntityAlreadyExistsException_WhenCartAlreadyExists()
        {
            // Arrange
            var userId = _faker.Random.Guid();
            var command = new CreateCartCommand { UserId = userId };
            var existingCart = new Cart { Id = Guid.NewGuid(), UserId = userId }; 

            _cartRepositoryMock.Setup(repo => repo.GetUserCartAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingCart); 

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<EntityAlreadyExistsException>();
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenCommandIsInvalid()
        {
            // Arrange
            var command = new CreateCartCommand()
            {
                UserId = Guid.Empty
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.UserId);
        }
    }
}

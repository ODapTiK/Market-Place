using Bogus;
using FluentAssertions;
using FluentValidation.TestHelper;
using Moq;

namespace OrderService
{
    public class GetUserCartQueryTests
    {
        private readonly Mock<ICartRepository> _cartRepositoryMock;
        private readonly GetUserCartQueryHandler _handler;
        private readonly Faker<Cart> _cartFaker;
        private readonly GetUserCartQueryValidator _validator;

        public GetUserCartQueryTests()
        {
            _cartRepositoryMock = new Mock<ICartRepository>();
            _handler = new GetUserCartQueryHandler(_cartRepositoryMock.Object);
            _validator = new GetUserCartQueryValidator();

            _cartFaker = new Faker<Cart>()
                .RuleFor(c => c.Id, f => f.Random.Guid())
                .RuleFor(c => c.UserId, f => f.Random.Guid())
                .RuleFor(c => c.Products, f => f.Make(f.Random.Int(1, 10), () => f.Random.Guid()));
        }

        [Fact]
        public async Task Handle_ShouldReturnCart_WhenCartExists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var cart = _cartFaker.Generate(); 
            var query = new GetUserCartQuery { UserId = userId };

            _cartRepositoryMock.Setup(repo => repo.GetUserCartAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(cart); 

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(cart);
            _cartRepositoryMock.Verify(repo => repo.GetUserCartAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowEntityNotFoundException_WhenCartDoesNotExist()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var query = new GetUserCartQuery { UserId = userId};

            _cartRepositoryMock.Setup(repo => repo.GetUserCartAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Cart?)null);

            // Act
            var act = async () => await _handler.Handle(query, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<EntityNotFoundException>();
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenCommandIsInvalid()
        {
            // Arrange
            var command = new GetUserCartQuery()
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

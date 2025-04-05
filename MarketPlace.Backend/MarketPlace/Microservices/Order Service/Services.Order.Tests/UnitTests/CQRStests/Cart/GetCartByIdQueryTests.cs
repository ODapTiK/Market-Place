using Bogus;
using FluentAssertions;
using FluentValidation.TestHelper;
using Moq;

namespace OrderService
{
    public class GetCartByIdQueryTests
    {
        private readonly Mock<ICartRepository> _cartRepositoryMock;
        private readonly GetCartByIdQueryHandler _handler;
        private readonly Faker<Cart> _cartFaker;
        private readonly GetCartByIdQueryValidator _validator;

        public GetCartByIdQueryTests()
        {
            _cartRepositoryMock = new Mock<ICartRepository>();
            _handler = new GetCartByIdQueryHandler(_cartRepositoryMock.Object);
            _validator = new GetCartByIdQueryValidator();

            _cartFaker = new Faker<Cart>()
                .RuleFor(c => c.Id, f => f.Random.Guid())
                .RuleFor(c => c.UserId, f => f.Random.Guid())
                .RuleFor(c => c.Products, f => f.Make(f.Random.Int(1, 10), () => f.Random.Guid())); 
        }

        [Fact]
        public async Task Handle_ShouldReturnCart_WhenCartExists()
        {
            // Arrange
            var cart = _cartFaker.Generate(); 
            var query = new GetCartByIdQuery { Id = cart.Id };

            _cartRepositoryMock.Setup(repo => repo.GetByIdAsync(cart.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(cart); 

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(cart);
            _cartRepositoryMock.Verify(repo => repo.GetByIdAsync(cart.Id, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowEntityNotFoundException_WhenCartDoesNotExist()
        {
            // Arrange
            var cartId = Guid.NewGuid();
            var query = new GetCartByIdQuery { Id = cartId };

            _cartRepositoryMock.Setup(repo => repo.GetByIdAsync(cartId, It.IsAny<CancellationToken>()))
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
            var command = new GetCartByIdQuery()
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

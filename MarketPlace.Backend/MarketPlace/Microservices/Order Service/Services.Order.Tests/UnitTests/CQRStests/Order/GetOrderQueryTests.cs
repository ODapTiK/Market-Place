using Bogus;
using FluentAssertions;
using Moq;
using FluentValidation.TestHelper;

namespace OrderService
{
    public class GetOrderQueryTests
    {
        private readonly Mock<IOrderRepository> _orderRepositoryMock;
        private readonly GetOrderQueryHandler _handler;
        private readonly Faker _faker;
        private readonly GetOrderQueryValidator _validator;

        public GetOrderQueryTests()
        {
            _orderRepositoryMock = new Mock<IOrderRepository>();
            _handler = new GetOrderQueryHandler(_orderRepositoryMock.Object);
            _faker = new Faker();
            _validator = new GetOrderQueryValidator();
        }

        [Fact]
        public async Task Handle_ShouldReturnOrder_WhenOrderExists()
        {
            // Arrange
            var orderId = _faker.Random.Guid();
            var query = new GetOrderQuery { Id = orderId };
            var existingOrder = new Order 
            { 
                Id = orderId, 
                TotalPrice = _faker.Random.Double() 
            }; 

            _orderRepositoryMock.Setup(repo => repo.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingOrder);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(existingOrder);
            _orderRepositoryMock.Verify(repo => repo.GetByIdAsync(orderId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowEntityNotFoundException_WhenOrderDoesNotExist()
        {
            // Arrange
            var orderId = _faker.Random.Guid();
            var query = new GetOrderQuery { Id = orderId };

            _orderRepositoryMock.Setup(repo => repo.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Order?)null); 

            // Act
            var act = async () => await _handler.Handle(query, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<EntityNotFoundException>();
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenCommandIsInvalid()
        {
            // Arrange
            var command = new GetOrderQuery
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

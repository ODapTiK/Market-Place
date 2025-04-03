using Bogus;
using Moq;
using FluentAssertions;
using FluentValidation.TestHelper;

namespace OrderService
{
    public class GetUserOrdersQueryTests
    {
        private readonly Mock<IOrderRepository> _orderRepositoryMock;
        private readonly GetUserOrdersQueryHandler _handler;
        private readonly Faker _faker;
        private readonly GetUserOrdersQueryValidator _validator;

        public GetUserOrdersQueryTests()
        {
            _orderRepositoryMock = new Mock<IOrderRepository>();
            _handler = new GetUserOrdersQueryHandler(_orderRepositoryMock.Object);
            _faker = new Faker();
            _validator = new GetUserOrdersQueryValidator();
        }

        [Fact]
        public async Task Handle_ShouldReturnListOfOrders_WhenUserHasOrders()
        {
            // Arrange
            var userId = _faker.Random.Guid();
            var query = new GetUserOrdersQuery { UserId = userId };
            var orders = _faker.Make(3, () => new Order { UserId = userId }); 

            _orderRepositoryMock.Setup(repo => repo.GetUserOrdersAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((List<Order>)orders);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(3);
            result.Should().BeEquivalentTo(orders);
            _orderRepositoryMock.Verify(repo => repo.GetUserOrdersAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyList_WhenUserHasNoOrders()
        {
            // Arrange
            var userId = _faker.Random.Guid();
            var query = new GetUserOrdersQuery { UserId = userId };
            var orders = new List<Order>();

            _orderRepositoryMock.Setup(repo => repo.GetUserOrdersAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(orders);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            _orderRepositoryMock.Verify(repo => repo.GetUserOrdersAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenCommandIsInvalid()
        {
            // Arrange
            var command = new GetUserOrdersQuery
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

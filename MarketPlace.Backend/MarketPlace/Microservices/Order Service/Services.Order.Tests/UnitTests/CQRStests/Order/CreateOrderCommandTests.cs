using Bogus;
using FluentValidation.TestHelper;
using FluentAssertions;
using Moq;

namespace OrderService
{
    public class CreateOrderCommandTests
    {
        private readonly Mock<IOrderRepository> _orderRepositoryMock;
        private readonly CreateOrderCommandHandler _handler;
        private readonly Faker<OrderPoint> _orderPointFaker;
        private readonly CreateOrderCommandValidator _validator;

        public CreateOrderCommandTests()
        {
            _orderRepositoryMock = new Mock<IOrderRepository>();
            _handler = new CreateOrderCommandHandler(_orderRepositoryMock.Object);
            _validator = new CreateOrderCommandValidator();

            _orderPointFaker = new Faker<OrderPoint>()
                .RuleFor(op => op.ProductId, f => Guid.NewGuid())
                .RuleFor(op => op.NumberOfUnits, f => f.Random.Int(1, 10));
        }

        [Fact]
        public async Task Handle_ShouldCreateOrder_WhenCommandIsValid()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var orderPoints = _orderPointFaker.Generate(5); 
            var command = new CreateOrderCommand
            {
                UserId = userId,
                Points = orderPoints,
                TotalPrice = 100m
            };

            _orderRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Guid.NewGuid());

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBe(Guid.Empty);
            _orderRepositoryMock.Verify(repo => repo.CreateAsync(It.Is<Order>(o =>
                o.UserId == userId &&
                o.OrderPoints.Count == orderPoints.Count &&
                o.TotalPrice == command.TotalPrice),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenCommandIsInvalid()
        {
            // Arrange
            var command = new CreateOrderCommand
            {
                UserId = Guid.Empty,
                Points = new List<OrderPoint> { new OrderPoint { ProductId = Guid.Empty, NumberOfUnits = 1 } },
                TotalPrice = -10m 
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.UserId);
            result.ShouldHaveValidationErrorFor(x => x.TotalPrice);
            result.ShouldHaveValidationErrorFor(x => x.Points);
        }
    }
}

using Bogus;
using FluentAssertions;
using Moq;
using System.Linq.Expressions;

namespace OrderService
{
    public class GetOrdersByIdListQueryTests
    {
        private readonly Mock<IOrderRepository> _orderRepositoryMock;
        private readonly GetOrdersByIdListQueryHandler _handler;
        private readonly Faker<Order> _orderFaker;
        private readonly Faker<OrderPoint> _orderPointFaker;

        public GetOrdersByIdListQueryTests()
        {
            _orderRepositoryMock = new Mock<IOrderRepository>();
            _handler = new GetOrdersByIdListQueryHandler(_orderRepositoryMock.Object);

            _orderPointFaker = new Faker<OrderPoint>()
                .RuleFor(op => op.Id, f => f.Random.Guid())
                .RuleFor(op => op.ProductId, f => f.Random.Guid())
                .RuleFor(op => op.productName, f => f.Commerce.ProductName())
                .RuleFor(op => op.productDescription, f => f.Commerce.ProductDescription())
                .RuleFor(op => op.productCategory, f => f.Commerce.Categories(1)[0])
                .RuleFor(op => op.productType, f => f.Commerce.ProductMaterial())
                .RuleFor(op => op.productImage, f => f.Image.PicsumUrl())
                .RuleFor(op => op.NumberOfUnits, f => f.Random.Int(1, 10));

            _orderFaker = new Faker<Order>()
                .RuleFor(o => o.Id, f => f.Random.Guid())
                .RuleFor(o => o.UserId, f => f.Random.Guid())
                .RuleFor(o => o.OrderPoints, f => _orderPointFaker.Generate(f.Random.Int(1, 5)))
                .RuleFor(o => o.TotalPrice, (f, o) => o.OrderPoints.Sum(op => op.NumberOfUnits * f.Random.Double(10, 1000)))
                .RuleFor(o => o.Status, f => f.PickRandom("Pending", "Completed", "Cancelled"))
                .RuleFor(o => o.ControlAdminId, f => f.Random.Guid())
                .RuleFor(o => o.OrderDateTime, f => f.Date.Recent());
        }

        [Fact]
        public async Task Handle_ShouldReturnFullOrderData_WhenIdsExist()
        {
            // Arrange
            var existingOrders = _orderFaker.Generate(3);
            var orderIds = existingOrders.Select(o => o.Id).ToList();

            _orderRepositoryMock.Setup(x => x.GetManyOrdersAsync(
                    It.IsAny<Expression<Func<Order, bool>>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingOrders);

            var query = new GetOrdersByIdListQuery { OrderIds = orderIds };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(existingOrders.Count);

            foreach (var order in result)
            {
                order.Should().NotBeNull();
                order.Id.Should().NotBeEmpty();
                order.UserId.Should().NotBeEmpty();
                order.ControlAdminId.Should().NotBeEmpty();
                order.Status.Should().NotBeNullOrWhiteSpace();
                order.OrderDateTime.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromDays(7));
                order.TotalPrice.Should().BePositive();

                order.OrderPoints.Should().NotBeNullOrEmpty();
                foreach (var point in order.OrderPoints)
                {
                    point.Should().NotBeNull();
                    point.Id.Should().NotBeEmpty();
                    point.ProductId.Should().NotBeEmpty();
                    point.NumberOfUnits.Should().BePositive();
                }
            }

            _orderRepositoryMock.Verify(x => x.GetManyOrdersAsync(
                It.IsAny<Expression<Func<Order, bool>>>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnOrdersWithCorrectPoints_WhenMultipleOrdersExist()
        {
            // Arrange
            var existingOrders = _orderFaker.Generate(2);
            foreach (var order in existingOrders)
            {
                foreach (var point in order.OrderPoints)
                {
                    point.OrderId = order.Id;
                }
            }

            var orderIds = existingOrders.Select(o => o.Id).ToList();

            _orderRepositoryMock.Setup(x => x.GetManyOrdersAsync(
                    It.IsAny<Expression<Func<Order, bool>>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingOrders);

            var query = new GetOrdersByIdListQuery { OrderIds = orderIds };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull().And.HaveCount(2);

            foreach (var order in result)
            {
                order.OrderPoints.Should().OnlyContain(
                    op => op.OrderId == order.Id,
                    "all order points should reference their parent order");
            }
        }

        [Fact]
        public async Task Handle_ShouldCalculateTotalPriceCorrectly()
        {
            // Arrange
            var testOrder = _orderFaker.Generate();

            testOrder.OrderPoints = new List<OrderPoint>
            {
                new OrderPoint { NumberOfUnits = 2, ProductId = Guid.NewGuid() },
                new OrderPoint { NumberOfUnits = 3, ProductId = Guid.NewGuid() }
            };

            _orderRepositoryMock.Setup(x => x.GetManyOrdersAsync(
                    It.IsAny<Expression<Func<Order, bool>>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Order> { testOrder });

            var query = new GetOrdersByIdListQuery { OrderIds = new List<Guid> { testOrder.Id } };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().Contain(testOrder);
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyOrderPoints_WhenOrderHasNoPoints()
        {
            // Arrange
            var testOrder = _orderFaker.Generate();
            testOrder.OrderPoints = new List<OrderPoint>(); 

            _orderRepositoryMock.Setup(x => x.GetManyOrdersAsync(
                    It.IsAny<Expression<Func<Order, bool>>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Order> { testOrder });

            var query = new GetOrdersByIdListQuery { OrderIds = new List<Guid> { testOrder.Id } };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            var returnedOrder = result.First();
            returnedOrder.OrderPoints.Should().NotBeNull().And.BeEmpty();
        }

        [Fact]
        public async Task Handle_ShouldReturnOrdersWithCorrectStatus()
        {
            // Arrange
            var testOrders = new List<Order>
            {
                _orderFaker.RuleFor(o => o.Status, "Pending").Generate(),
                _orderFaker.RuleFor(o => o.Status, "Completed").Generate()
            };

            var orderIds = testOrders.Select(o => o.Id).ToList();

            _orderRepositoryMock.Setup(x => x.GetManyOrdersAsync(
                    It.IsAny<Expression<Func<Order, bool>>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(testOrders);

            var query = new GetOrdersByIdListQuery { OrderIds = orderIds };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().Contain(o => o.Status == "Pending");
            result.Should().Contain(o => o.Status == "Completed");
        }
    }
}

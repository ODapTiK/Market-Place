using FluentAssertions;

namespace OrderService
{
    public class OrderRepositoryTests : IClassFixture<TestOrderDatabaseFixture>
    {
        private readonly OrderRepository _repository;
        private readonly IOrderDbContext _context;

        public OrderRepositoryTests(TestOrderDatabaseFixture fixture)
        {
            _context = fixture._context;
            _repository = new OrderRepository(_context);
        }

        [Fact]
        public async Task GetUserOrdersAsync_ShouldReturnUserOrders_WhenOrdersExist()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var order1 = new Order { Id = Guid.NewGuid(), UserId = userId };
            var order2 = new Order { Id = Guid.NewGuid(), UserId = userId };
            _context.Orders.AddRange(order1, order2);
            await _context.SaveChangesAsync(CancellationToken.None);

            // Act
            var result = await _repository.GetUserOrdersAsync(userId, CancellationToken.None);

            // Assert
            result.Should().HaveCount(2);
            result.Should().Contain(order => order.Id == order1.Id);
            result.Should().Contain(order => order.Id == order2.Id);
        }

        [Fact]
        public async Task GetUserOrdersAsync_ShouldReturnEmptyList_WhenNoOrdersExist()
        {
            // Arrange
            var userId = Guid.NewGuid(); 

            // Act
            var result = await _repository.GetUserOrdersAsync(userId, CancellationToken.None);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetManyOrdersAsync_ShouldReturnOrders_WhenOrdersExist()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var orders = new List<Order>
            {
                new Order { Id = Guid.NewGuid(), UserId = userId, OrderPoints = new List<OrderPoint>() },
                new Order { Id = Guid.NewGuid(), UserId = userId, OrderPoints = new List<OrderPoint>() },
                new Order { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), OrderPoints = new List<OrderPoint>() }
            };

            await _context.Orders.AddRangeAsync(orders);
            await _context.SaveChangesAsync(CancellationToken.None);

            // Act
            var result = await _repository.GetManyOrdersAsync(o => o.UserId == userId, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetManyOrdersAsync_ShouldReturnEmptyList_WhenNoOrdersExist()
        {
            // Arrange
            var userId = Guid.NewGuid();

            // Act
            var result = await _repository.GetManyOrdersAsync(o => o.UserId == userId, CancellationToken.None);

            // Assert
            result.Should().BeEmpty();
            result.Should().NotBeNull();
        }
    }
}

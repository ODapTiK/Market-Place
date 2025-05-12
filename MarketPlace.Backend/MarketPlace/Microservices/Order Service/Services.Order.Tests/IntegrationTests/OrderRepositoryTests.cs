using Bogus;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace OrderService
{
    [Collection("Database Collection")]
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

        [Fact]
        public async Task DeleteUserOrdersAsync_ShouldDeleteAllUserOrders()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var otherUserId = Guid.NewGuid();

            var userOrders = new Faker<Order>()
                .RuleFor(o => o.Id, f => Guid.NewGuid())
                .RuleFor(o => o.UserId, userId)
                .Generate(3);

            var otherUserOrders = new Faker<Order>()
                .RuleFor(o => o.Id, f => Guid.NewGuid())
                .RuleFor(o => o.UserId, otherUserId)
                .Generate(2);

            await _context.Orders.AddRangeAsync(userOrders);
            await _context.Orders.AddRangeAsync(otherUserOrders);
            await _context.SaveChangesAsync(CancellationToken.None);

            // Act
            await _repository.DeleteUserOrdersAsync(userId, CancellationToken.None);

            // Assert
            var remainingUserOrders = await _context.Orders
                .Where(o => o.UserId == userId)
                .ToListAsync(CancellationToken.None);

            var remainingOtherUserOrders = await _context.Orders
                .Where(o => o.UserId == otherUserId)
                .ToListAsync(CancellationToken.None);

            remainingUserOrders.Should().BeEmpty();
            remainingOtherUserOrders.Should().HaveCount(2);
        }

        [Fact]
        public async Task DeleteUserOrdersAsync_ShouldDoNothing_WhenUserHasNoOrders()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var initialOrderCount = await _context.Orders.CountAsync(CancellationToken.None);

            // Act
            await _repository.DeleteUserOrdersAsync(userId, CancellationToken.None);

            // Assert
            var finalOrderCount = await _context.Orders.CountAsync(CancellationToken.None);
            finalOrderCount.Should().Be(initialOrderCount);
        }

        [Fact]
        public async Task DeleteUserOrdersAsync_ShouldWork_WhenNoOrdersExist()
        {
            // Arrange
            _context.Orders.RemoveRange(await _context.Orders.ToListAsync());
            await _context.SaveChangesAsync(CancellationToken.None);
            var userId = Guid.NewGuid();

            // Act
            await _repository.DeleteUserOrdersAsync(userId, CancellationToken.None);

            // Assert
            var orders = await _context.Orders.ToListAsync(CancellationToken.None);
            orders.Should().BeEmpty();
        }
    }
}

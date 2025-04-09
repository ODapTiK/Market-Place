using FluentAssertions;
using System.Linq.Expressions;

namespace UserService
{
    [Collection("Database Collection")]
    public class UserRepositoryTests : IClassFixture<TestUserDatabaseFixture>
    {
        private readonly UserRepository _userRepository;
        private readonly IUserDbContext _context;

        public UserRepositoryTests(TestUserDatabaseFixture fixture)
        {
            _context = fixture._context;
            _userRepository = new UserRepository(_context);
        }

        [Fact]
        public async Task AddOrderAsync_ShouldAddOrderIdToUser()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                UserOrdersId = new List<Guid>()
            };
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync(CancellationToken.None);

            var orderId = Guid.NewGuid();

            // Act
            await _userRepository.AddOrderAsync(user, orderId, CancellationToken.None);

            // Assert
            var updatedUser = await _context.Users.FindAsync(userId);
            updatedUser.UserOrdersId.Should().Contain(orderId);
        }

        [Fact]
        public async Task RemoveOrderAsync_ShouldRemoveOrderIdFromUser()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var orderId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                UserOrdersId = new List<Guid> { orderId }
            };
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync(CancellationToken.None);

            // Act
            await _userRepository.RemoveOrderAsync(user, orderId, CancellationToken.None);

            // Assert
            var updatedUser = await _context.Users.FindAsync(userId);
            updatedUser.UserOrdersId.Should().NotContain(orderId);
        }

        [Fact]
        public async Task GetManyAsync_ShouldReturnUsers_WhenPredicateMatches()
        {
            // Arrange
            var users = new List<User>
            {
                new User { Id = Guid.NewGuid(), Name = "Alice" },
                new User { Id = Guid.NewGuid(), Name = "Bob" },
                new User { Id = Guid.NewGuid(), Name = "Charlie" }
            };

            await _context.Users.AddRangeAsync(users);
            await _context.SaveChangesAsync(default);

            // Act
            var predicate = (Expression<Func<User, bool>>)(u => u.Name.StartsWith("A") || u.Name.StartsWith("C"));
            var result = await _userRepository.GetManyAsync(predicate, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.Should().Contain(u => u.Name == "Alice");
            result.Should().Contain(u => u.Name == "Charlie");
        }

        [Fact]
        public async Task GetManyAsync_ShouldReturnEmptyList_WhenNoUsersMatchPredicate()
        {
            // Arrange
            var users = new List<User>
            {
                new User { Id = Guid.NewGuid(), Name = "Alice" },
                new User { Id = Guid.NewGuid(), Name = "Bob" }
            };

            await _context.Users.AddRangeAsync(users);
            await _context.SaveChangesAsync(default);

            // Act
            var predicate = (Expression<Func<User, bool>>)(u => u.Name.StartsWith("D"));
            var result = await _userRepository.GetManyAsync(predicate, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

    }
}

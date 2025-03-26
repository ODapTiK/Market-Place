using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace UserService
{
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
    }
}

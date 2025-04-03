using Bogus;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;

namespace AuthorizationService
{
    public class UserRepositoryTests : IClassFixture<TestUserDatabaseFixture>
    {
        private readonly UserRepository _userRepository;
        private readonly AuthDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly Faker _faker;

        public UserRepositoryTests(TestUserDatabaseFixture fixture)
        {
            _context = fixture._context;
            _userManager = fixture._userManager;
            _userRepository = new UserRepository(_userManager);
            _faker = new Faker();
        }

        [Fact]
        public async Task CreateAsync_ShouldCreateUser()
        {
            // Arrange
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = _faker.Internet.Email(),
                UserName = Guid.NewGuid().ToString(),
                PasswordHash = new PasswordHasher<User>().HashPassword(null, "Password123")
            };

            // Act
            var userId = await _userRepository.CreateAsync(user, CancellationToken.None);

            // Assert
            var createdUser = await _userRepository.FindByIdAsync(userId, CancellationToken.None);
            createdUser.Should().NotBeNull();
            createdUser.Email.Should().Be(user.Email);
        }

        [Fact]
        public async Task FindByEmailAsync_ShouldReturnUser()
        {
            // Arrange
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = _faker.Internet.Email(),
                UserName = Guid.NewGuid().ToString(),
                PasswordHash = new PasswordHasher<User>().HashPassword(null, "Password123")
            };

            await _userRepository.CreateAsync(user, CancellationToken.None);

            // Act
            var foundUser = await _userRepository.FindByEmailAsync(user.Email, CancellationToken.None);

            // Assert
            foundUser.Should().NotBeNull();
            foundUser.Email.Should().Be(user.Email);
        }

        [Fact]
        public async Task FindByIdAsync_ShouldReturnUser()
        {
            // Arrange
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = _faker.Internet.Email(),
                UserName = Guid.NewGuid().ToString(),
                PasswordHash = new PasswordHasher<User>().HashPassword(null, "Password123")
            };

            await _userRepository.CreateAsync(user, CancellationToken.None);

            // Act
            var foundUser = await _userRepository.FindByIdAsync(user.Id, CancellationToken.None);

            // Assert
            foundUser.Should().NotBeNull();
            foundUser.Id.Should().Be(user.Id);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveUser()
        {
            // Arrange
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = _faker.Internet.Email(),
                UserName = Guid.NewGuid().ToString(),
                PasswordHash = new PasswordHasher<User>().HashPassword(null, "Password123")
            };

            await _userRepository.CreateAsync(user, CancellationToken.None);

            // Act
            await _userRepository.DeleteAsync(user, CancellationToken.None);

            // Assert
            var deletedUser = await _userRepository.FindByIdAsync(user.Id, CancellationToken.None);
            deletedUser.Should().BeNull();
        }

        [Fact]
        public async Task AddUserToRoleAsync_ShouldAddRoleToUser()
        {
            // Arrange
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = _faker.Internet.Email(),
                UserName = Guid.NewGuid().ToString(),
                PasswordHash = new PasswordHasher<User>().HashPassword(null, "Password123")
            };

            await _userRepository.CreateAsync(user, CancellationToken.None);
            var roleName = "Admin";

            // Act
            var result = await _userRepository.AddUserToRoleAsync(user, roleName);

            // Assert
            result.Succeeded.Should().BeTrue();
            var roles = await _userRepository.GetUserRoleAsync(user);
            roles.Should().Contain(roleName);
        }

        [Fact]
        public async Task GetUserRolesAsync_ShouldReturnUserRoles()
        {
            // Arrange
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = _faker.Internet.Email(),
                UserName = Guid.NewGuid().ToString(),
                PasswordHash = new PasswordHasher<User>().HashPassword(null, "Password123")
            };

            await _userRepository.CreateAsync(user, CancellationToken.None);
            var roleName = "Admin";
            await _userRepository.AddUserToRoleAsync(user, roleName);

            // Act
            var roles = await _userRepository.GetUserRoleAsync(user);

            // Assert
            roles.Should().Contain(roleName);
        }
    }
}
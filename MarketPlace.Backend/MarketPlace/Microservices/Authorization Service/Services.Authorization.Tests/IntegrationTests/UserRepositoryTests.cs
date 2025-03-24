using Bogus;
using FluentAssertions;

namespace AuthorizationService
{
    public class UserRepositoryTests : IClassFixture<TestUserDatabaseFixture>
    {
        private readonly UserRepository _userRepository;
        private readonly IAuthDbContext _context;

        public UserRepositoryTests(TestUserDatabaseFixture fixture)
        {
            _context = fixture._context;
            _userRepository = new UserRepository(_context);
        }

        [Fact]
        public async Task CreateAsync_ShouldAddUser()
        {
            // Arrange
            var faker = new Faker();

            var newUser = new User()
            { 
                Id = Guid.NewGuid(), 
                Password = faker.Internet.Password(),
                Email = faker.Internet.Email(),
                Role = "User"
            };

            // Act
            var userId = await _userRepository.CreateAsync(newUser, CancellationToken.None);

            // Assert
            userId.Should().Be(newUser.Id);

            // Проверяем, добавлен ли пользователь в базу данных
            var addedUser = await _context.Users.FindAsync(userId);
            addedUser.Should().NotBeNull();
            addedUser.Email.Should().Be(newUser.Email);
            addedUser.Password.Should().Be(newUser.Password);
            addedUser.Role.Should().Be(newUser.Role);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveUser()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var userToDelete = new User { Id = userId };

            _context.Users.Add(userToDelete);
            await _context.SaveChangesAsync(CancellationToken.None); 

            // Act
            await _userRepository.DeleteAsync(userToDelete, CancellationToken.None);

            // Assert
            var deletedUser = await _context.Users.FindAsync(userId);
            deletedUser.Should().BeNull(); 
        }

        [Fact]
        public async Task FindByIdAsync_ShouldReturnUser()
        {
            // Arrange
            var faker = new Faker();

            var userId = Guid.NewGuid();
            var expectedUser = new User 
            { 
                Id = userId, 
                Password = faker.Internet.Password(), 
                Email = faker.Internet.Email(),
                Role = "User"
            };
            await _context.Users.AddAsync(expectedUser);
            await _context.SaveChangesAsync(CancellationToken.None);

            // Act
            var result = await _userRepository.FindByIdAsync(userId, CancellationToken.None);

            // Assert
            result.Should().NotBeNull(); 
            result.Should().BeEquivalentTo(expectedUser); 
        }

        [Fact]
        public async Task FindByEmailAsync_ShouldReturnUser()
        {
            // Arrange
            var faker = new Faker();

            var userId = Guid.NewGuid();
            var email = faker.Internet.Email();

            var expectedUser = new User
            {
                Id = userId,
                Password = faker.Internet.Password(),
                Email = email,
                Role = "User"
            };

            await _context.Users.AddAsync(expectedUser);
            await _context.SaveChangesAsync(CancellationToken.None);

            // Act
            var result = await _userRepository.FindByEmailAsync(email, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedUser);
        }
    }
}
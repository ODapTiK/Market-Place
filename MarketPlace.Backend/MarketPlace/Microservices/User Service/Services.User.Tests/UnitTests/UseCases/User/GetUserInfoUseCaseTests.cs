using FluentAssertions;
using Moq;

namespace UserService
{
    public class GetUserInfoUseCaseTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly GetUserInfoUseCase _getUserInfoUseCase;

        public GetUserInfoUseCaseTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _getUserInfoUseCase = new GetUserInfoUseCase(_userRepositoryMock.Object);
        }

        [Fact]
        public async Task Execute_ShouldReturnUser_WhenUserIdIsValid()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User { Id = userId, Name = "Test User" };

            _userRepositoryMock.Setup(u => u.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            // Act
            var result = await _getUserInfoUseCase.Execute(userId, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(user);
        }

        [Fact]
        public async Task Execute_ShouldThrowValidationException_WhenUserIdIsEmpty()
        {
            // Arrange
            var userId = Guid.Empty;

            // Act
            var act = async () => await _getUserInfoUseCase.Execute(userId, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<FluentValidation.ValidationException>();
        }

        [Fact]
        public async Task Execute_ShouldThrowEntityNotFoundException_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = Guid.NewGuid();

            _userRepositoryMock.Setup(u => u.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((User?)null); 

            // Act
            var act = async () => await _getUserInfoUseCase.Execute(userId, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<EntityNotFoundException>();
        }
    }
}

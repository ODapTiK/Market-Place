using FluentAssertions;
using Moq;

namespace UserService
{
    public class DeleteUserUseCaseTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly DeleteUserUseCase _deleteUserUseCase;

        public DeleteUserUseCaseTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _deleteUserUseCase = new DeleteUserUseCase(_userRepositoryMock.Object);
        }

        [Fact]
        public async Task Execute_ShouldDeleteUser_WhenUserIdIsValid()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User { Id = userId };

            _userRepositoryMock.Setup(u => u.GetByIdAsync(userId, CancellationToken.None))
                .ReturnsAsync(user);

            // Act
            await _deleteUserUseCase.Execute(userId);

            // Assert
            _userRepositoryMock.Verify(u => u.DeleteAsync(user, CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task Execute_ShouldThrowValidationException_WhenUserIdIsEmpty()
        {
            // Arrange
            var userId = Guid.Empty;

            // Act
            Func<Task> act = async () => await _deleteUserUseCase.Execute(userId);

            // Assert
            await act.Should().ThrowAsync<FluentValidation.ValidationException>();
        }

        [Fact]
        public async Task Execute_ShouldThrowEntityNotFoundException_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = Guid.NewGuid();

            _userRepositoryMock.Setup(u => u.GetByIdAsync(userId, CancellationToken.None))
                .ReturnsAsync((User?)null); 

            // Act
            Func<Task> act = async () => await _deleteUserUseCase.Execute(userId);

            // Assert
            await act.Should().ThrowAsync<EntityNotFoundException>();
        }
    }
}

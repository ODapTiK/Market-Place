using FluentAssertions;
using Moq;

namespace AuthorizationService
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
        public async Task Handle_WhenUserExists_DeletesUser()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var request = new DeleteUserRequest 
            { 
                Id = userId 
            };

            var user = new User 
            { 
                Id = userId 
            };  

            _userRepositoryMock.Setup(repo => repo.FindByIdAsync(userId, CancellationToken.None))
                               .ReturnsAsync(user);

            // Act
            await _deleteUserUseCase.Handle(request, CancellationToken.None);

            // Assert
            _userRepositoryMock.Verify(repo => repo.DeleteAsync(user, CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenIdIsEmpty_ThrowsValidationException()
        {
            // Arrange
            var request = new DeleteUserRequest 
            { 
                Id = Guid.Empty 
            };

            // Act
            Func<Task> act = async () => await _deleteUserUseCase.Handle(request, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<FluentValidation.ValidationException>();
        }

        [Fact]
        public async Task Handle_WhenUserDoesNotExist_ThrowsEntityNotFoundException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var request = new DeleteUserRequest 
            { 
                Id = userId 
            };

            _userRepositoryMock.Setup(repo => repo.FindByIdAsync(userId, CancellationToken.None))
                               .ReturnsAsync((User?)null);

            // Act
            Func<Task> act = async () => await _deleteUserUseCase.Handle(request, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<EntityNotFoundException>();
        }
    }
}

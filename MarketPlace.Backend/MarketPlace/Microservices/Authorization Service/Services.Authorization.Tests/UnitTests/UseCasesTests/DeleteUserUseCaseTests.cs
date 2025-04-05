using FluentAssertions;
using Moq;
using Proto.AuthUser;

namespace AuthorizationService
{
    public class DeleteUserUseCaseTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly DeleteUserUseCase _deleteUserUseCase;
        private readonly Mock<AuthUserService.AuthUserServiceClient> _userServiceClientMock;

        public DeleteUserUseCaseTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _userServiceClientMock = new Mock<AuthUserService.AuthUserServiceClient>();
            _deleteUserUseCase = new DeleteUserUseCase(_userRepositoryMock.Object, _userServiceClientMock.Object);
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
                Id = userId,
            };  

            _userRepositoryMock.Setup(repo => repo.FindByIdAsync(userId, CancellationToken.None))
                .ReturnsAsync(user);
            _userRepositoryMock.Setup(repo => repo.GetUserRoleAsync(It.IsAny<User>()))
                .ReturnsAsync(["User"]);

            var mockCall = CallHelpers.CreateAsyncUnaryCall(new DeleteEntityResponse
            {
                Message = "Test",
                Success = true
            });
            _userServiceClientMock
                .Setup(m => m.DeleteEntityAsync(
                    It.IsAny<DeleteEntityRequest>(), null, null, CancellationToken.None))
                .Returns(mockCall);

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
            var act = async () => await _deleteUserUseCase.Handle(request, CancellationToken.None);

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
            var act = async () => await _deleteUserUseCase.Handle(request, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<EntityNotFoundException>();
        }

        [Fact]
        public async Task Handle_ShouldThrowGRPCRequestFailException_WhenGrpcServiceReturnsFailure()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var request = new DeleteUserRequest
            {
                Id = userId
            };

            var user = new User
            {
                Id = userId,
            };

            _userRepositoryMock.Setup(repo => repo.FindByIdAsync(userId, CancellationToken.None))
                .ReturnsAsync(user);
            _userRepositoryMock.Setup(repo => repo.GetUserRoleAsync(It.IsAny<User>()))
                .ReturnsAsync(["User"]);

            var mockCall = CallHelpers.CreateAsyncUnaryCall(new DeleteEntityResponse
            {
                Message = "Failure test",
                Success = false
            });
            _userServiceClientMock
                .Setup(m => m.DeleteEntityAsync(
                    It.IsAny<DeleteEntityRequest>(), null, null, CancellationToken.None))
                .Returns(mockCall);

            // Act
            var act = async () => await _deleteUserUseCase.Handle(request, CancellationToken.None);
            
            // Assert
            await act.Should().ThrowAsync<GRPCRequestFailException>().WithMessage("Failure test");
        }
    }
}

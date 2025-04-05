using FluentAssertions;
using Moq;
using Proto.OrderUser;

namespace UserService
{
    public class DeleteUserUseCaseTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly DeleteUserUseCase _deleteUserUseCase;
        private readonly Mock<OrderUserService.OrderUserServiceClient> _orderUserServiceMock;

        public DeleteUserUseCaseTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _orderUserServiceMock = new Mock<OrderUserService.OrderUserServiceClient>();
            _deleteUserUseCase = new DeleteUserUseCase(_userRepositoryMock.Object, _orderUserServiceMock.Object);
        }

        [Fact]
        public async Task Execute_ShouldDeleteUser_WhenUserIdIsValid()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User { Id = userId };

            _userRepositoryMock.Setup(u => u.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            var mockCall = CallHelpers.CreateAsyncUnaryCall(new Response
            {
                Message = "Test",
                Success = true
            });
            _orderUserServiceMock
                .Setup(m => m.DeleteCartAsync(
                    It.IsAny<CartRequest>(), null, null, CancellationToken.None))
                .Returns(mockCall);
            _orderUserServiceMock
                .Setup(m => m.DeleteUserOrdersAsync(
                    It.IsAny<DeleteUserOrdersRequest>(), null, null, CancellationToken.None))
                .Returns(mockCall);

            // Act
            await _deleteUserUseCase.Execute(userId, CancellationToken.None);

            // Assert
            _userRepositoryMock.Verify(u => u.DeleteAsync(user, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Execute_ShouldThrowGRPCRequestFailException_WhenGRPCRequestFailed()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User { Id = userId };

            _userRepositoryMock.Setup(u => u.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            var mockCall = CallHelpers.CreateAsyncUnaryCall(new Response
            {
                Message = "Fail",
                Success = false
            });
            _orderUserServiceMock
                .Setup(m => m.DeleteCartAsync(
                    It.IsAny<CartRequest>(), null, null, CancellationToken.None))
                .Returns(mockCall);
            _orderUserServiceMock
                .Setup(m => m.DeleteUserOrdersAsync(
                    It.IsAny<DeleteUserOrdersRequest>(), null, null, CancellationToken.None))
                .Returns(mockCall);

            // Act
            var act = async () => await _deleteUserUseCase.Execute(userId, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<GRPCRequestFailException>().WithMessage("Fail");
        }

        [Fact]
        public async Task Execute_ShouldThrowValidationException_WhenUserIdIsEmpty()
        {
            // Arrange
            var userId = Guid.Empty;

            // Act
            var act = async () => await _deleteUserUseCase.Execute(userId, CancellationToken.None);

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
            var act = async () => await _deleteUserUseCase.Execute(userId, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<EntityNotFoundException>();
        }
    }
}

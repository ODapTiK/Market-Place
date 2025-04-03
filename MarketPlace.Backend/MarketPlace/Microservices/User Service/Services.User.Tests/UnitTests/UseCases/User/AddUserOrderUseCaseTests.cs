using FluentAssertions;
using Moq;

namespace UserService
{
    public class AddUserOrderUseCaseTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly AddUserOrderUseCase _addUserOrderUseCase;

        public AddUserOrderUseCaseTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _addUserOrderUseCase = new AddUserOrderUseCase(_userRepositoryMock.Object);
        }

        [Fact]
        public async Task Execute_ShouldAddOrderToUser_WhenUserAndOrderIdsAreValid()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var orderId = Guid.NewGuid();
            var user = new User { Id = userId };

            _userRepositoryMock.Setup(u => u.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            // Act
            await _addUserOrderUseCase.Execute(userId, orderId, CancellationToken.None);

            // Assert
            _userRepositoryMock.Verify(u => u.AddOrderAsync(user, orderId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Execute_ShouldThrowValidationException_WhenUserIdIsEmpty()
        {
            // Arrange
            var orderId = Guid.NewGuid();

            // Act
            var act = async () => await _addUserOrderUseCase.Execute(Guid.Empty, orderId, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<FluentValidation.ValidationException>();
        }

        [Fact]
        public async Task Execute_ShouldThrowValidationException_WhenOrderIdIsEmpty()
        {
            // Arrange
            var userId = Guid.NewGuid();

            // Act
            var act = async () => await _addUserOrderUseCase.Execute(userId, Guid.Empty, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<FluentValidation.ValidationException>();
        }

        [Fact]
        public async Task Execute_ShouldThrowEntityNotFoundException_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var orderId = Guid.NewGuid();

            _userRepositoryMock.Setup(u => u.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((User?)null); 

            // Act
            var act = async () => await _addUserOrderUseCase.Execute(userId, orderId, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<EntityNotFoundException>();
        }
    }
}

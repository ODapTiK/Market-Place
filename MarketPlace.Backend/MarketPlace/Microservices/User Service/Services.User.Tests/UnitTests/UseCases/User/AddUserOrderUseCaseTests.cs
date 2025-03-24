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

            _userRepositoryMock.Setup(u => u.GetByIdAsync(userId, CancellationToken.None))
                .ReturnsAsync(user);

            // Act
            await _addUserOrderUseCase.Execute(userId, orderId);

            // Assert
            _userRepositoryMock.Verify(u => u.AddOrderAsync(user, orderId, CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task Execute_ShouldThrowValidationException_WhenUserIdIsEmpty()
        {
            // Arrange
            var orderId = Guid.NewGuid();

            // Act
            Func<Task> act = async () => await _addUserOrderUseCase.Execute(Guid.Empty, orderId);

            // Assert
            await act.Should().ThrowAsync<FluentValidation.ValidationException>();
        }

        [Fact]
        public async Task Execute_ShouldThrowValidationException_WhenOrderIdIsEmpty()
        {
            // Arrange
            var userId = Guid.NewGuid();

            // Act
            Func<Task> act = async () => await _addUserOrderUseCase.Execute(userId, Guid.Empty);

            // Assert
            await act.Should().ThrowAsync<FluentValidation.ValidationException>();
        }

        [Fact]
        public async Task Execute_ShouldThrowEntityNotFoundException_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var orderId = Guid.NewGuid();

            _userRepositoryMock.Setup(u => u.GetByIdAsync(userId, CancellationToken.None))
                .ReturnsAsync((User?)null); 

            // Act
            Func<Task> act = async () => await _addUserOrderUseCase.Execute(userId, orderId);

            // Assert
            await act.Should().ThrowAsync<EntityNotFoundException>();
        }
    }
}

using FluentAssertions;
using Moq;

namespace UserService
{
    public class RemoveUserOrderUseCaseTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly RemoveUserOrderUseCase _removeUserOrderUseCase;

        public RemoveUserOrderUseCaseTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _removeUserOrderUseCase = new RemoveUserOrderUseCase(_userRepositoryMock.Object);
        }

        [Fact]
        public async Task Execute_ShouldRemoveOrder_WhenUserIdAndOrderIdAreValid()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var orderId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                UserOrdersId = new List<Guid> { orderId }
            };

            _userRepositoryMock.Setup(u => u.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            // Act
            await _removeUserOrderUseCase.Execute(userId, orderId, CancellationToken.None);

            // Assert
            _userRepositoryMock.Verify(u => u.RemoveOrderAsync(user, orderId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Execute_ShouldThrowValidationException_WhenUserIdIsEmpty()
        {
            // Arrange
            var userId = Guid.Empty;
            var orderId = Guid.NewGuid();

            // Act
            var act = async () => await _removeUserOrderUseCase.Execute(userId, orderId, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<FluentValidation.ValidationException>();
        }

        [Fact]
        public async Task Execute_ShouldThrowValidationException_WhenOrderIdIsEmpty()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var orderId = Guid.Empty;

            // Act
            var act = async () => await _removeUserOrderUseCase.Execute(userId, orderId, CancellationToken.None);

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
            var act = async () => await _removeUserOrderUseCase.Execute(userId, orderId, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<EntityNotFoundException>();
        }

        [Fact]
        public async Task Execute_ShouldThrowEntityNotFoundException_WhenOrderDoesNotBelongToUser()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var orderId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                UserOrdersId = new List<Guid> { Guid.NewGuid() } 
            };

            _userRepositoryMock.Setup(u => u.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            // Act
            var act = async () => await _removeUserOrderUseCase.Execute(userId, orderId, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<EntityNotFoundException>();
        }
    }
}

using Moq;

namespace UserService
{
    public class RemoveControlAdminOrderUseCaseTests
    {
        private readonly Mock<IAdminRepository> _adminRepositoryMock;
        private readonly RemoveControlAdminOrderUseCase _useCase;

        public RemoveControlAdminOrderUseCaseTests()
        {
            _adminRepositoryMock = new Mock<IAdminRepository>();
            _useCase = new RemoveControlAdminOrderUseCase(_adminRepositoryMock.Object);
        }

        [Fact]
        public async Task Execute_AdminExists_RemovesOrderIdFromAdmin()
        {
            // Arrange
            var adminId = Guid.NewGuid();
            var orderId = Guid.NewGuid();
            var admin = new Admin
            {
                Id = adminId,
                AdminControlOrdersId = new List<Guid> { orderId }
            };

            _adminRepositoryMock
                .Setup(repo => repo.GetByIdAsync(adminId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(admin);

            // Act
            await _useCase.Execute(adminId, orderId, CancellationToken.None);

            // Assert
            Assert.DoesNotContain(orderId, admin.AdminControlOrdersId);
            _adminRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Execute_AdminDoesNotExist_ThrowsEntityNotFoundException()
        {
            // Arrange
            var adminId = Guid.NewGuid();
            var orderId = Guid.NewGuid();

            _adminRepositoryMock
                .Setup(repo => repo.GetByIdAsync(adminId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Admin?)null); 

            // Act & Assert
            var exception = await Assert.ThrowsAsync<EntityNotFoundException>(() =>
                _useCase.Execute(adminId, orderId, CancellationToken.None));
        }

        [Fact]
        public async Task Execute_OrderIdNotInAdminControlOrders_DoesNotThrow()
        {
            // Arrange
            var adminId = Guid.NewGuid();
            var orderId = Guid.NewGuid();
            var admin = new Admin
            {
                Id = adminId,
                AdminControlOrdersId = new List<Guid>() 
            };

            _adminRepositoryMock
                .Setup(repo => repo.GetByIdAsync(adminId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(admin);

            // Act
            await _useCase.Execute(adminId, orderId, CancellationToken.None);

            // Assert
            _adminRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}

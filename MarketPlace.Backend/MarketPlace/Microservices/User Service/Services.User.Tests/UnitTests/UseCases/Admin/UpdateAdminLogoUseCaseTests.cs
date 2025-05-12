using Moq;

namespace UserService
{
    public class UpdateAdminLogoUseCaseTests
    {
        private readonly Mock<IAdminRepository> _adminRepositoryMock;
        private readonly UpdateAdminLogoUseCase _useCase;

        public UpdateAdminLogoUseCaseTests()
        {
            _adminRepositoryMock = new Mock<IAdminRepository>();
            _useCase = new UpdateAdminLogoUseCase(_adminRepositoryMock.Object);
        }

        [Fact]
        public async Task Execute_AdminExists_UpdatesLogo()
        {
            // Arrange
            var adminId = Guid.NewGuid();
            var base64Image = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAUA...";
            var admin = new Admin
            {
                Id = adminId,
                Logo = null 
            };

            _adminRepositoryMock
                .Setup(repo => repo.GetByIdAsync(adminId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(admin);

            // Act
            await _useCase.Execute(adminId, base64Image, CancellationToken.None);

            // Assert
            Assert.Equal(base64Image, admin.Logo);
            _adminRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Execute_AdminDoesNotExist_ThrowsEntityNotFoundException()
        {
            // Arrange
            var adminId = Guid.NewGuid();
            var base64Image = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAUA...";

            _adminRepositoryMock
                .Setup(repo => repo.GetByIdAsync(adminId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Admin?)null); 

            // Act & Assert
            var exception = await Assert.ThrowsAsync<EntityNotFoundException>(() =>
                _useCase.Execute(adminId, base64Image, CancellationToken.None));
        }
    }
}

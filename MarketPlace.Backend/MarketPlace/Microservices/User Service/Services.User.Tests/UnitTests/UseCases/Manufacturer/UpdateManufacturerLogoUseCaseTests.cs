using Moq;

namespace UserService
{
    public class UpdateManufacturerLogoUseCaseTests
    {
        private readonly Mock<IManufacturerRepository> _manufacturerRepositoryMock;
        private readonly UpdateManufacturerLogoUseCase _useCase;

        public UpdateManufacturerLogoUseCaseTests()
        {
            _manufacturerRepositoryMock = new Mock<IManufacturerRepository>();
            _useCase = new UpdateManufacturerLogoUseCase(_manufacturerRepositoryMock.Object);
        }

        [Fact]
        public async Task Execute_ManufacturerExists_UpdatesLogo()
        {
            // Arrange
            var manufacturerId = Guid.NewGuid();
            var base64Image = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAUA...";
            var manufacturer = new Manufacturer
            {
                Id = manufacturerId,
                Logo = null
            };

            _manufacturerRepositoryMock
                .Setup(repo => repo.GetByIdAsync(manufacturerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(manufacturer);

            // Act
            await _useCase.Execute(manufacturerId, base64Image, CancellationToken.None);

            // Assert
            Assert.Equal(base64Image, manufacturer.Logo);
            _manufacturerRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Execute_ManufacturerDoesNotExist_ThrowsEntityNotFoundException()
        {
            // Arrange
            var manufacturerId = Guid.NewGuid();
            var base64Image = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAUA...";

            _manufacturerRepositoryMock
                .Setup(repo => repo.GetByIdAsync(manufacturerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Manufacturer?)null); 

            // Act & Assert
            var exception = await Assert.ThrowsAsync<EntityNotFoundException>(() =>
                _useCase.Execute(manufacturerId, base64Image, CancellationToken.None));
        }
    }
}

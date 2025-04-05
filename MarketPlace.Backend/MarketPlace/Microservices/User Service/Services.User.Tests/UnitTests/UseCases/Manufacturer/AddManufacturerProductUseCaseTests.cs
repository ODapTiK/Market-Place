using FluentAssertions;
using Moq;

namespace UserService
{
    public class AddManufacturerProductUseCaseTests
    {
        private readonly Mock<IManufacturerRepository> _manufacturerRepositoryMock;
        private readonly AddManufacturerProductUseCase _addManufacturerProductUseCase;

        public AddManufacturerProductUseCaseTests()
        {
            _manufacturerRepositoryMock = new Mock<IManufacturerRepository>();
            _addManufacturerProductUseCase = new AddManufacturerProductUseCase(_manufacturerRepositoryMock.Object);
        }

        [Fact]
        public async Task Execute_ShouldThrowValidationException_WhenManufacturerIdIsEmpty()
        {
            // Arrange
            var productId = Guid.NewGuid();

            // Act
            var act = async () => await _addManufacturerProductUseCase.Execute(Guid.Empty, productId, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<FluentValidation.ValidationException>();
        }

        [Fact]
        public async Task Execute_ShouldThrowValidationException_WhenProductIdIsEmpty()
        {
            // Arrange
            var manufacturerId = Guid.NewGuid();

            // Act
            var act = async () => await _addManufacturerProductUseCase.Execute(manufacturerId, Guid.Empty, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<FluentValidation.ValidationException>();
        }

        [Fact]
        public async Task Execute_ShouldThrowEntityNotFoundException_WhenManufacturerDoesNotExist()
        {
            // Arrange
            var manufacturerId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            _manufacturerRepositoryMock.Setup(m => m.GetByIdAsync(manufacturerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Manufacturer?)null); 

            // Act
            var act = async () => await _addManufacturerProductUseCase.Execute(manufacturerId, productId, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<EntityNotFoundException>();
        }

        [Fact]
        public async Task Execute_ShouldAddProductToManufacturer_WhenManufacturerExists()
        {
            // Arrange
            var manufacturerId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var manufacturer = new Manufacturer { Id = manufacturerId, Organization = "Test Manufacturer" };

            _manufacturerRepositoryMock.Setup(m => m.GetByIdAsync(manufacturerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(manufacturer);

            // Act
            await _addManufacturerProductUseCase.Execute(manufacturerId, productId, CancellationToken.None);

            // Assert
            _manufacturerRepositoryMock.Verify(m => m.AddProductAsync(manufacturer, productId, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}

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
            Func<Task> act = async () => await _addManufacturerProductUseCase.Execute(Guid.Empty, productId);

            // Assert
            await act.Should().ThrowAsync<FluentValidation.ValidationException>();
        }

        [Fact]
        public async Task Execute_ShouldThrowValidationException_WhenProductIdIsEmpty()
        {
            // Arrange
            var manufacturerId = Guid.NewGuid();

            // Act
            Func<Task> act = async () => await _addManufacturerProductUseCase.Execute(manufacturerId, Guid.Empty);

            // Assert
            await act.Should().ThrowAsync<FluentValidation.ValidationException>();
        }

        [Fact]
        public async Task Execute_ShouldThrowEntityNotFoundException_WhenManufacturerDoesNotExist()
        {
            // Arrange
            var manufacturerId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            _manufacturerRepositoryMock.Setup(m => m.GetByIdAsync(manufacturerId, CancellationToken.None))
                .ReturnsAsync((Manufacturer?)null); 

            // Act
            Func<Task> act = async () => await _addManufacturerProductUseCase.Execute(manufacturerId, productId);

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

            _manufacturerRepositoryMock.Setup(m => m.GetByIdAsync(manufacturerId, CancellationToken.None))
                .ReturnsAsync(manufacturer);

            // Act
            await _addManufacturerProductUseCase.Execute(manufacturerId, productId);

            // Assert
            _manufacturerRepositoryMock.Verify(m => m.AddProductAsync(manufacturer, productId, CancellationToken.None), Times.Once);
        }
    }
}

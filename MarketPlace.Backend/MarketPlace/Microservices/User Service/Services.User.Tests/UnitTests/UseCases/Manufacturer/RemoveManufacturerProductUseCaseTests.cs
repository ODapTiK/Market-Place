using FluentAssertions;
using Moq;

namespace UserService
{
    public class RemoveManufacturerProductUseCaseTests
    {
        private readonly Mock<IManufacturerRepository> _manufacturerRepositoryMock;
        private readonly RemoveManufacturerProductUseCase _removeManufacturerProductUseCase;

        public RemoveManufacturerProductUseCaseTests()
        {
            _manufacturerRepositoryMock = new Mock<IManufacturerRepository>();
            _removeManufacturerProductUseCase = new RemoveManufacturerProductUseCase(_manufacturerRepositoryMock.Object);
        }

        [Fact]
        public async Task Execute_ShouldThrowValidationException_WhenManufacturerIdIsEmpty()
        {
            // Arrange
            var manufacturerId = Guid.Empty;
            var productId = Guid.NewGuid();

            // Act
            var act = async () => await _removeManufacturerProductUseCase.Execute(manufacturerId, productId, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<FluentValidation.ValidationException>();
        }

        [Fact]
        public async Task Execute_ShouldThrowValidationException_WhenProductIdIsEmpty()
        {
            // Arrange
            var manufacturerId = Guid.NewGuid();
            var productId = Guid.Empty;

            // Act
            var act = async () => await _removeManufacturerProductUseCase.Execute(manufacturerId, productId, CancellationToken.None);

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
            var act = async () => await _removeManufacturerProductUseCase.Execute(manufacturerId, productId, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<EntityNotFoundException>();
        }

        [Fact]
        public async Task Execute_ShouldThrowEntityNotFoundException_WhenProductDoesNotBelongToManufacturer()
        {
            // Arrange
            var manufacturerId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var manufacturer = new Manufacturer
            {
                Id = manufacturerId,
                OrganizationProductsId = new List<Guid> { Guid.NewGuid() } 
            };

            _manufacturerRepositoryMock.Setup(m => m.GetByIdAsync(manufacturerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(manufacturer);

            // Act
            var act = async () => await _removeManufacturerProductUseCase.Execute(manufacturerId, productId, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<EntityNotFoundException>();
        }

        [Fact]
        public async Task Execute_ShouldRemoveProduct_WhenManufacturerExistsAndProductBelongsToManufacturer()
        {
            // Arrange
            var manufacturerId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var manufacturer = new Manufacturer
            {
                Id = manufacturerId,
                OrganizationProductsId = new List<Guid> { productId } 
            };

            _manufacturerRepositoryMock.Setup(m => m.GetByIdAsync(manufacturerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(manufacturer);

            // Act
            await _removeManufacturerProductUseCase.Execute(manufacturerId, productId, CancellationToken.None);

            // Assert
            _manufacturerRepositoryMock.Verify(m => m.RemoveProductAsync(manufacturer, productId, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}

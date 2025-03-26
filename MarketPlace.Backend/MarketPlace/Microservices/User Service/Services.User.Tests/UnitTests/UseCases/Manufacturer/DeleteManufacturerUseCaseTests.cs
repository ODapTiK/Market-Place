using FluentAssertions;
using Moq;

namespace UserService
{
    public class DeleteManufacturerUseCaseTests
    {
        private readonly Mock<IManufacturerRepository> _manufacturerRepositoryMock;
        private readonly DeleteManufacturerUseCase _deleteManufacturerUseCase;

        public DeleteManufacturerUseCaseTests()
        {
            _manufacturerRepositoryMock = new Mock<IManufacturerRepository>();
            _deleteManufacturerUseCase = new DeleteManufacturerUseCase(_manufacturerRepositoryMock.Object);
        }

        [Fact]
        public async Task Execute_ShouldThrowValidationException_WhenManufacturerIdIsEmpty()
        {
            // Arrange
            var manufacturerId = Guid.Empty;

            // Act
            Func<Task> act = async () => await _deleteManufacturerUseCase.Execute(manufacturerId, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<FluentValidation.ValidationException>();
        }

        [Fact]
        public async Task Execute_ShouldThrowEntityNotFoundException_WhenManufacturerDoesNotExist()
        {
            // Arrange
            var manufacturerId = Guid.NewGuid();

            _manufacturerRepositoryMock.Setup(m => m.GetByIdAsync(manufacturerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Manufacturer?)null); 

            // Act
            Func<Task> act = async () => await _deleteManufacturerUseCase.Execute(manufacturerId, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<EntityNotFoundException>();
        }

        [Fact]
        public async Task Execute_ShouldDeleteManufacturer_WhenManufacturerExists()
        {
            // Arrange
            var manufacturerId = Guid.NewGuid();
            var manufacturer = new Manufacturer { Id = manufacturerId, Organization = "Test Manufacturer" };

            _manufacturerRepositoryMock.Setup(m => m.GetByIdAsync(manufacturerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(manufacturer);

            // Act
            await _deleteManufacturerUseCase.Execute(manufacturerId, CancellationToken.None);

            // Assert
            _manufacturerRepositoryMock.Verify(m => m.DeleteAsync(manufacturer, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}

using FluentAssertions;
using Moq;

namespace UserService
{
    public class GetManufacturerInfoUseCaseTests
    {
        private readonly Mock<IManufacturerRepository> _manufacturerRepositoryMock;
        private readonly GetManufacturerInfoUseCase _getManufacturerInfoUseCase;

        public GetManufacturerInfoUseCaseTests()
        {
            _manufacturerRepositoryMock = new Mock<IManufacturerRepository>();
            _getManufacturerInfoUseCase = new GetManufacturerInfoUseCase(_manufacturerRepositoryMock.Object);
        }

        [Fact]
        public async Task Execute_ShouldThrowEntityNotFoundException_WhenManufacturerDoesNotExist()
        {
            // Arrange
            var manufacturerId = Guid.NewGuid();

            _manufacturerRepositoryMock.Setup(m => m.GetByIdAsync(manufacturerId, CancellationToken.None))
                .ReturnsAsync((Manufacturer?)null); 

            // Act
            Func<Task> act = async () => await _getManufacturerInfoUseCase.Execute(manufacturerId);

            // Assert
            await act.Should().ThrowAsync<EntityNotFoundException>();
        }

        [Fact]
        public async Task Execute_ShouldReturnManufacturer_WhenManufacturerExists()
        {
            // Arrange
            var manufacturerId = Guid.NewGuid();
            var manufacturer = new Manufacturer { Id = manufacturerId, Organization = "Test Manufacturer" };

            _manufacturerRepositoryMock.Setup(m => m.GetByIdAsync(manufacturerId, CancellationToken.None))
                .ReturnsAsync(manufacturer); 

            // Act
            var result = await _getManufacturerInfoUseCase.Execute(manufacturerId);

            // Assert
            result.Should().BeEquivalentTo(manufacturer);
            _manufacturerRepositoryMock.Verify(m => m.GetByIdAsync(manufacturerId, CancellationToken.None), Times.Once());  
        }
    }
}

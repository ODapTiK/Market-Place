using FluentAssertions;
using Moq;

namespace UserService
{
    public class GetManufacturersIdUseCaseTests
    {
        private readonly Mock<IManufacturerRepository> _manufacturerRepositoryMock;
        private readonly GetManufacturersIdUseCase _useCase;

        public GetManufacturersIdUseCaseTests()
        {
            _manufacturerRepositoryMock = new Mock<IManufacturerRepository>();
            _useCase = new GetManufacturersIdUseCase(_manufacturerRepositoryMock.Object);
        }

        [Fact]
        public async Task Execute_ShouldReturnListOfManufacturerIds_WhenManufacturersExist()
        {
            // Arrange
            var manufacturers = new List<Manufacturer>
            {
                new Manufacturer { Id = Guid.NewGuid(), Organization = "Manufacturer 1" },
                new Manufacturer { Id = Guid.NewGuid(), Organization = "Manufacturer 2" }
            };

            _manufacturerRepositoryMock
                .Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(manufacturers);

            // Act
            var result = await _useCase.Execute(CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.Should().Contain(manufacturers.Select(m => m.Id));
        }

        [Fact]
        public async Task Execute_ShouldReturnEmptyList_WhenNoManufacturersExist()
        {
            // Arrange
            var manufacturers = new List<Manufacturer>();

            _manufacturerRepositoryMock
                .Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(manufacturers);

            // Act
            var result = await _useCase.Execute(CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }
    }
}

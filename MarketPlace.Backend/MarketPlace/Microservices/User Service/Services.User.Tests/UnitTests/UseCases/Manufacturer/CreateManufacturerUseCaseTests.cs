using AutoMapper;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace UserService
{
    public class CreateManufacturerUseCaseTests
    {
        private readonly Mock<IManufacturerRepository> _manufacturerRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IValidator<ManufacturerDTO>> _validatorMock;
        private readonly CreateManufacturerUseCase _createManufacturerUseCase;

        public CreateManufacturerUseCaseTests()
        {
            _manufacturerRepositoryMock = new Mock<IManufacturerRepository>();
            _mapperMock = new Mock<IMapper>();
            _validatorMock = new Mock<IValidator<ManufacturerDTO>>();
            _createManufacturerUseCase = new CreateManufacturerUseCase(_manufacturerRepositoryMock.Object, _mapperMock.Object, _validatorMock.Object);
        }

        [Fact]
        public async Task Execute_ShouldThrowValidationException_WhenManufacturerDTOIsInvalid()
        {
            // Arrange
            var manufacturerDTO = new ManufacturerDTO(); 
            var validationErrors = new List<ValidationFailure>
            {
                new ValidationFailure("Organization", "Organization is required")
            };

            _validatorMock.Setup(v => v.ValidateAsync(manufacturerDTO, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult(validationErrors));

            // Act
            var act = async () => await _createManufacturerUseCase.Execute(manufacturerDTO, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ValidationException>();
        }

        [Fact]
        public async Task Execute_ShouldCreateManufacturer_WhenManufacturerDTOIsValid()
        {
            // Arrange
            var manufacturerDTO = new ManufacturerDTO { Organization = "Valid Manufacturer" };
            var manufacturer = new Manufacturer { Id = Guid.NewGuid(), Organization = manufacturerDTO.Organization };

            _validatorMock.Setup(v => v.ValidateAsync(manufacturerDTO, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _mapperMock.Setup(m => m.Map<Manufacturer>(manufacturerDTO))
                .Returns(manufacturer);

            _manufacturerRepositoryMock.Setup(m => m.CreateAsync(manufacturer, It.IsAny<CancellationToken>()))
                .ReturnsAsync(manufacturer.Id);

            // Act
            var result = await _createManufacturerUseCase.Execute(manufacturerDTO, CancellationToken.None);

            // Assert
            result.Should().Be(manufacturer.Id);
            _manufacturerRepositoryMock.Verify(m => m.CreateAsync(manufacturer, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}

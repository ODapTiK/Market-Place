using AutoMapper;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace UserService
{
    public class UpdateManufacturerUseCaseTests
    {
        private readonly Mock<IManufacturerRepository> _manufacturerRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IValidator<ManufacturerDTO>> _validatorMock;
        private readonly UpdateManufacturerUseCase _updateManufacturerUseCase;

        public UpdateManufacturerUseCaseTests()
        {
            _manufacturerRepositoryMock = new Mock<IManufacturerRepository>();
            _mapperMock = new Mock<IMapper>();
            _validatorMock = new Mock<IValidator<ManufacturerDTO>>();
            _updateManufacturerUseCase = new UpdateManufacturerUseCase(_manufacturerRepositoryMock.Object, _mapperMock.Object, _validatorMock.Object);
        }

        [Fact]
        public async Task Execute_ShouldUpdateManufacturer_WhenManufacturerDTOIsValid()
        {
            // Arrange
            var manufacturerId = Guid.NewGuid();
            var manufacturerDTO = new ManufacturerDTO { Id = manufacturerId, Organization = "Updated Manufacturer" };
            var manufacturer = new Manufacturer { Id = manufacturerId, Organization = "Old Manufacturer" };

            _validatorMock.Setup(v => v.ValidateAsync(manufacturerDTO, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _manufacturerRepositoryMock.Setup(m => m.GetByIdAsync(manufacturerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(manufacturer);

            _mapperMock.Setup(m => m.Map<Manufacturer>(manufacturerDTO))
                .Returns(manufacturer);

            // Act
            await _updateManufacturerUseCase.Execute(manufacturerDTO, CancellationToken.None);

            // Assert
            _manufacturerRepositoryMock.Verify(m => m.UpdateAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Execute_ShouldThrowValidationException_WhenManufacturerDTOIsInvalid()
        {
            // Arrange
            var manufacturerDTO = new ManufacturerDTO { Id = Guid.NewGuid() }; 
            var validationErrors = new List<ValidationFailure>
            {
                new ValidationFailure("Name", "Name is required")
            };

            _validatorMock.Setup(v => v.ValidateAsync(manufacturerDTO, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult(validationErrors));

            // Act
            var act = async () => await _updateManufacturerUseCase.Execute(manufacturerDTO, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ValidationException>();
        }

        [Fact]
        public async Task Execute_ShouldThrowEntityNotFoundException_WhenManufacturerDoesNotExist()
        {
            // Arrange
            var manufacturerId = Guid.NewGuid();
            var manufacturerDTO = new ManufacturerDTO { Id = manufacturerId, Organization = "Updated Manufacturer" };

            _validatorMock.Setup(v => v.ValidateAsync(manufacturerDTO, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _manufacturerRepositoryMock.Setup(m => m.GetByIdAsync(manufacturerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Manufacturer?)null); 

            // Act
            var act = async () => await _updateManufacturerUseCase.Execute(manufacturerDTO, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<EntityNotFoundException>();
        }
    }
}

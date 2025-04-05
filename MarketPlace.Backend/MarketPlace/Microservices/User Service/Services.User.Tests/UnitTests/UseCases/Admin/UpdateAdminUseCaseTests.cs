using AutoMapper;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace UserService
{
    public class UpdateAdminUseCaseTests
    {
        private readonly Mock<IAdminRepository> _adminRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IValidator<AdminDTO>> _validatorMock;
        private readonly UpdateAdminUseCase _updateAdminUseCase;

        public UpdateAdminUseCaseTests()
        {
            _adminRepositoryMock = new Mock<IAdminRepository>();
            _mapperMock = new Mock<IMapper>();
            _validatorMock = new Mock<IValidator<AdminDTO>>();
            _updateAdminUseCase = new UpdateAdminUseCase(_adminRepositoryMock.Object, _mapperMock.Object, _validatorMock.Object);
        }

        [Fact]
        public async Task Execute_ShouldThrowValidationException_WhenAdminDTOIsInvalid()
        {
            // Arrange
            var adminDTO = new AdminDTO { Id = Guid.NewGuid() };
            var validationResult = new ValidationResult([new ValidationFailure("Name", "Name is required")]);
            _validatorMock.Setup(v => v.ValidateAsync(adminDTO, It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);

            // Act
            var act = async () => await _updateAdminUseCase.Execute(adminDTO, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ValidationException>();
        }

        [Fact]
        public async Task Execute_ShouldThrowEntityNotFoundException_WhenAdminDoesNotExist()
        {
            // Arrange
            var adminDTO = new AdminDTO { Id = Guid.NewGuid() };
            _validatorMock.Setup(v => v.ValidateAsync(adminDTO, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult()); 
            _adminRepositoryMock.Setup(a => a.GetByIdAsync(adminDTO.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Admin?)null); 

            // Act
            var act = async () => await _updateAdminUseCase.Execute(adminDTO, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<EntityNotFoundException>();
        }

        [Fact]
        public async Task Execute_ShouldUpdateAdmin_WhenAdminExists()
        {
            // Arrange
            var adminId = Guid.NewGuid();
            var adminDTO = new AdminDTO { Id = adminId, Name = "Updated Admin" };
            var existingAdmin = new Admin { Id = adminId, Name = "Old Admin" };

            _validatorMock.Setup(v => v.ValidateAsync(adminDTO, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult()); 
            _adminRepositoryMock.Setup(a => a.GetByIdAsync(adminId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingAdmin); 
            _mapperMock.Setup(m => m.Map<Admin>(adminDTO))
                .Returns(new Admin { Id = adminId, Name = "Updated Admin" });

            // Act
            await _updateAdminUseCase.Execute(adminDTO, CancellationToken.None);

            // Assert
            _adminRepositoryMock.Verify(a => a.UpdateAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}

using AutoMapper;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace UserService
{
    public class CreateAdminUseCaseTests
    {
        private readonly Mock<IAdminRepository> _adminRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IValidator<AdminDTO>> _validatorMock;
        private readonly CreateAdminUseCase _createAdminUseCase;

        public CreateAdminUseCaseTests()
        {
            _adminRepositoryMock = new Mock<IAdminRepository>();
            _mapperMock = new Mock<IMapper>();
            _validatorMock = new Mock<IValidator<AdminDTO>>();
            _createAdminUseCase = new CreateAdminUseCase(_adminRepositoryMock.Object, _mapperMock.Object, _validatorMock.Object);
        }

        [Fact]
        public async Task Execute_ShouldThrowValidationException_WhenAdminDTOIsInvalid()
        {
            // Arrange
            var adminDTO = new AdminDTO();
            var validationResult = new ValidationResult([new ValidationFailure("Name", "Name is required")]);
            _validatorMock.Setup(v => v.ValidateAsync(adminDTO, CancellationToken.None))
                .ReturnsAsync(validationResult);

            // Act
            Func<Task> act = async () => await _createAdminUseCase.Execute(adminDTO);

            // Assert
            await act.Should().ThrowAsync<ValidationException>();
        }

        [Fact]
        public async Task Execute_ShouldCreateAdmin_WhenAdminDTOIsValid()
        {
            // Arrange
            var adminDTO = new AdminDTO { Name = "Admin Name" };
            var adminId = Guid.NewGuid();
            var admin = new Admin { Id = adminId };

            _validatorMock.Setup(v => v.ValidateAsync(adminDTO, CancellationToken.None))
                .ReturnsAsync(new ValidationResult());
            _mapperMock.Setup(m => m.Map<Admin>(adminDTO))
                .Returns(admin);
            _adminRepositoryMock.Setup(a => a.CreateAsync(admin, CancellationToken.None))
                .ReturnsAsync(adminId);

            // Act
            var result = await _createAdminUseCase.Execute(adminDTO);

            // Assert
            result.Should().Be(adminId);
            _adminRepositoryMock.Verify(a => a.CreateAsync(admin, CancellationToken.None), Times.Once);
            _mapperMock.Verify(m => m.Map<Admin>(adminDTO), Times.Once);
        }
    }
}

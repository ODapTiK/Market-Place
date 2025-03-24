using AutoMapper;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace UserService
{
    public class UpdateUserUseCaseTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IValidator<UserDTO>> _validatorMock;
        private readonly UpdateUserUseCase _updateUserUseCase;

        public UpdateUserUseCaseTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _mapperMock = new Mock<IMapper>();
            _validatorMock = new Mock<IValidator<UserDTO>>();
            _updateUserUseCase = new UpdateUserUseCase(_userRepositoryMock.Object, _mapperMock.Object, _validatorMock.Object);
        }

        [Fact]
        public async Task Execute_ShouldUpdateUser_WhenUserDTOIsValid()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var userDTO = new UserDTO { Id = userId, Name = "Updated User" };
            var user = new User { Id = userId, Name = "Old User" };

            _validatorMock.Setup(v => v.ValidateAsync(userDTO, CancellationToken.None))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());

            _userRepositoryMock.Setup(u => u.GetByIdAsync(userId, CancellationToken.None))
                .ReturnsAsync(user);

            _mapperMock.Setup(m => m.Map<User>(userDTO))
                .Returns(user);

            // Act
            await _updateUserUseCase.Execute(userDTO);

            // Assert
            _userRepositoryMock.Verify(u => u.UpdateAsync(CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task Execute_ShouldThrowValidationException_WhenUserDTOIsInvalid()
        {
            // Arrange
            var userDTO = new UserDTO { Id = Guid.NewGuid() }; 
            var validationErrors = new List<ValidationFailure>
            {
                new ValidationFailure("Name", "Name is required")
            };

            _validatorMock.Setup(v => v.ValidateAsync(userDTO, CancellationToken.None))
                .ReturnsAsync(new ValidationResult(validationErrors));

            // Act
            Func<Task> act = async () => await _updateUserUseCase.Execute(userDTO);

            // Assert
            await act.Should().ThrowAsync<ValidationException>();
        }

        [Fact]
        public async Task Execute_ShouldThrowEntityNotFoundException_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var userDTO = new UserDTO { Id = userId, Name = "Updated User" };

            _validatorMock.Setup(v => v.ValidateAsync(userDTO, CancellationToken.None))
                .ReturnsAsync(new ValidationResult());

            _userRepositoryMock.Setup(u => u.GetByIdAsync(userId, CancellationToken.None))
                .ReturnsAsync((User?)null); 

            // Act
            Func<Task> act = async () => await _updateUserUseCase.Execute(userDTO);

            // Assert
            await act.Should().ThrowAsync<EntityNotFoundException>();
        }
    }
}

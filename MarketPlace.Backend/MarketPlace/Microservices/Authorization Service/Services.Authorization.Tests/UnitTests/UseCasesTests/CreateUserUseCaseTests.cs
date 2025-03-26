using FluentAssertions;
using FluentValidation;
using Moq;
using FluentValidation.Results;
using Bogus;
using Microsoft.AspNetCore.Identity;

namespace AuthorizationService
{
    public class CreateUserUseCaseTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IRoleRepository> _roleRepositoryMock;
        private readonly Mock<IPasswordEncryptor> _passwordEncryptorMock;
        private readonly Mock<IValidator<UserDTO>> _validatorMock;
        private readonly CreateUserUseCase _createUserUseCase;

        public CreateUserUseCaseTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _roleRepositoryMock = new Mock<IRoleRepository>();
            _passwordEncryptorMock = new Mock<IPasswordEncryptor>();
            _validatorMock = new Mock<IValidator<UserDTO>>();

            _createUserUseCase = new CreateUserUseCase(
                _userRepositoryMock.Object,
                _passwordEncryptorMock.Object,
                _validatorMock.Object,
                _roleRepositoryMock.Object
            );
        }

        [Fact]
        public async Task Handle_ShouldCreateUser_WhenRequestIsValid()
        {
            // Arrange
            var faker = new Faker();
            var userDTO = new UserDTO
            {
                Email = faker.Internet.Email(),
                Password = faker.Internet.Password(),
                Role = "User"
            };

            var request = new CreateUserRequest(userDTO);

            var encryptedPassword = "hashedPassword";

            _validatorMock.Setup(v => v.ValidateAsync(userDTO, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());
            _passwordEncryptorMock.Setup(pe => pe.GenerateEncryptedPassword(userDTO.Password))
                .Returns(encryptedPassword);
            _userRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Guid.NewGuid());
            _userRepositoryMock.Setup(repo => repo.FindByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((User?)null);
            _userRepositoryMock.Setup(repo => repo.AddUserToRoleAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);
            _roleRepositoryMock.Setup(repo => repo.RoleExistsAsync(It.IsAny<string>()))
                .ReturnsAsync(true);

            // Act
            var result = await _createUserUseCase.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBe(Guid.Empty);
            _userRepositoryMock.Verify(repo => 
                repo.CreateAsync(It.Is<User>(u => u.Email == userDTO.Email && u.PasswordHash == encryptedPassword), 
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowValidationException_WhenValidationFails()
        {
            // Arrange
            var userDTO = new UserDTO
            {
                Email = "", 
                Password = "",
                Role = "User "
            };

            var request = new CreateUserRequest(userDTO);
            var validationErrors = new List<ValidationFailure>
            {
                new ValidationFailure("Email", "Email is required."),
                new ValidationFailure("Password", "Password is required.")
            };

            _validatorMock.Setup(v => v.ValidateAsync(userDTO, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult(validationErrors));

            // Act
            Func<Task> act = async () => await _createUserUseCase.Handle(request, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<FluentValidation.ValidationException>();
        }

        [Fact]
        public async Task Handle_WhenUserWithEmailExists_ThrowsEntityAlreadyExistsException()
        {
            // Arrange
            var userDto = new UserDTO
            {
                Email = "existing@example.com",
                Password = "password123",
                Role = "User "
            };

            var request = new CreateUserRequest(userDto);

            _validatorMock.Setup(v => v.ValidateAsync(userDto, It.IsAny<CancellationToken>()))
                          .ReturnsAsync(new FluentValidation.Results.ValidationResult());
            _userRepositoryMock.Setup(repo => repo.FindByEmailAsync(userDto.Email, CancellationToken.None))
                               .ReturnsAsync(new User()); 

            // Act
            Func<Task> act = async () => await _createUserUseCase.Handle(request, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<EntityAlreadyExistsException>();
        }
    }
}


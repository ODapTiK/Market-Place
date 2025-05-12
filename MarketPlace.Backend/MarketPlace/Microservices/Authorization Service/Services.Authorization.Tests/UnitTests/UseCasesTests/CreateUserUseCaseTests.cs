using FluentAssertions;
using FluentValidation;
using Moq;
using FluentValidation.Results;
using Bogus;
using Microsoft.AspNetCore.Identity;
using Proto.AuthUser;
using System.Data;
using Grpc.Core;

namespace AuthorizationService
{
    public class CreateUserUseCaseTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IRoleRepository> _roleRepositoryMock;
        //private readonly Mock<IPasswordEncryptor> _passwordEncryptorMock;
        private readonly Mock<IValidator<UserDTO>> _validatorMock;
        private readonly Mock<AuthUserService.AuthUserServiceClient> _userServiceClientMock;
        private readonly CreateUserUseCase _createUserUseCase;

        public CreateUserUseCaseTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _roleRepositoryMock = new Mock<IRoleRepository>();
            //_passwordEncryptorMock = new Mock<IPasswordEncryptor>();
            _validatorMock = new Mock<IValidator<UserDTO>>();
            _userServiceClientMock = new Mock<AuthUserService.AuthUserServiceClient>();

            _createUserUseCase = new CreateUserUseCase(
                _userRepositoryMock.Object,
                //_passwordEncryptorMock.Object,
                _validatorMock.Object,
                _roleRepositoryMock.Object,
                _userServiceClientMock.Object
            );
        }

        [Fact]
        public async Task Handle_ShouldCreateUser_WhenRequestIsValid()
        {
            // Arrange
            var faker = new Faker();
            var userDTO = new CreateUserDTO
            {
                Email = faker.Internet.Email(),
                Password = faker.Internet.Password(),
                Role = "User",
                Name = "name",
                Surname = "surname",
                BirthDate = DateTime.UtcNow.AddDays(-20)
            };

            var request = new CreateUserRequest(userDTO);

            _validatorMock.Setup(v => v.ValidateAsync(userDTO, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());
            //_passwordEncryptorMock.Setup(pe => pe.GenerateEncryptedPassword(userDTO.Password))
                //.Returns(encryptedPassword);
            _userRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Guid.NewGuid());
            _userRepositoryMock.Setup(repo => repo.FindByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((User?)null);
            _userRepositoryMock.Setup(repo => repo.AddUserToRoleAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);
            _roleRepositoryMock.Setup(repo => repo.RoleExistsAsync(It.IsAny<string>()))
                .ReturnsAsync(true);

            var mockCall = CallHelpers.CreateAsyncUnaryCall(new CreateUserResponse 
            { 
                Message = "Test",
                Success = true
            });
            _userServiceClientMock
                .Setup(m => m.CreateUserAsync(
                    It.IsAny<Proto.AuthUser.CreateUserRequest>(), null, null, CancellationToken.None))
                .Returns(mockCall);

            // Act
            var result = await _createUserUseCase.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBe(Guid.Empty);
            _userRepositoryMock.Verify(repo => 
                repo.CreateAsync(It.Is<User>(u => u.Email == userDTO.Email && u.PasswordHash == userDTO.Password), 
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
                Role = "User"
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
            var act = async () => await _createUserUseCase.Handle(request, CancellationToken.None);

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
                Role = "User"
            };

            var request = new CreateUserRequest(userDto);

            _validatorMock.Setup(v => v.ValidateAsync(userDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());
            _userRepositoryMock.Setup(repo => repo.FindByEmailAsync(userDto.Email, CancellationToken.None))
                .ReturnsAsync(new User()); 

            // Act
            var act = async () => await _createUserUseCase.Handle(request, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<EntityAlreadyExistsException>();
        }

        [Fact]
        public async Task Handle_ShouldThrowEntityNotFoundException_WhenRoleDoesNotExist()
        {
            // Arrange
            var userDto = new CreateUserDTO
            {
                Email = "user@example.com",
                Password = "Password123",
                Role = "NonExistentRole"
            };

            var request = new CreateUserRequest(userDto);
            _validatorMock.Setup(v => v.ValidateAsync(userDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());
            _userRepositoryMock.Setup(repo => repo.FindByEmailAsync(userDto.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync((User?)null);
            _roleRepositoryMock.Setup(repo => repo.RoleExistsAsync(userDto.Role))
                .ReturnsAsync(false);

            // Act
            var act = async () => await _createUserUseCase.Handle(request, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<EntityNotFoundException>();
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenRoleAssignmentFails()
        {
            // Arrange
            var userDto = new CreateUserDTO
            {
                Email = "user@example.com",
                Password = "Password123",
                Role = "User "
            };

            var request = new CreateUserRequest(userDto);
            _validatorMock.Setup(v => v.ValidateAsync(userDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());
            _userRepositoryMock.Setup(repo => repo.FindByEmailAsync(userDto.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync((User?)null);
            _roleRepositoryMock.Setup(repo => repo.RoleExistsAsync(userDto.Role))
                .ReturnsAsync(true);
            _userRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Guid.NewGuid());
            _userRepositoryMock.Setup(repo => repo.AddUserToRoleAsync(It.IsAny<User>(), userDto.Role))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Role assignment failed." }));

            // Act
            var act = async () => await _createUserUseCase.Handle(request, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("Role assignment failed: Role assignment failed.");
        }

        [Fact]
        public async Task Handle_ShouldThrowGRPCRequestFailException_WhenGrpcServiceReturnsFailure()
        {
            // Arrange
            var userDTO = new CreateUserDTO
            {
                Email = "user@example.com",
                Password = "Password123",
                Role = "User",
                BirthDate = DateTime.UtcNow.AddYears(-30)
            };

            var encryptedPassword = "encryptedPassword";

            _validatorMock.Setup(v => v.ValidateAsync(userDTO, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());
            //_passwordEncryptorMock.Setup(pe => pe.GenerateEncryptedPassword(userDTO.Password))
                //.Returns(encryptedPassword);
            _userRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Guid.NewGuid());
            _userRepositoryMock.Setup(repo => repo.FindByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((User?)null);
            _userRepositoryMock.Setup(repo => repo.AddUserToRoleAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);
            _roleRepositoryMock.Setup(repo => repo.RoleExistsAsync(It.IsAny<string>()))
                .ReturnsAsync(true);

            var mockCall = CallHelpers.CreateAsyncUnaryCall(new CreateUserResponse
            {
                Message = "Failure test",
                Success = false
            });
            _userServiceClientMock
                .Setup(m => m.CreateUserAsync(
                    It.IsAny<Proto.AuthUser.CreateUserRequest>(), null, null, CancellationToken.None))
                .Returns(mockCall);

            // Act
            var act = async () => await _createUserUseCase.Handle(new CreateUserRequest(userDTO), CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<GRPCRequestFailException>().WithMessage("Failure test");
        }
    }
}


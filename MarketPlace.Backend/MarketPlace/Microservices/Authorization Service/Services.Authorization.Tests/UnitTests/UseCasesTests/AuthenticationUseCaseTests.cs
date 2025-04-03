using FluentAssertions;
using FluentValidation;
using Moq;
using FluentValidation.Results;
using System.Security.Authentication;

namespace AuthorizationService
{
    public class AuthenticationUseCaseTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IValidator<AuthUserDTO>> _validatorMock;
        private readonly Mock<IPasswordEncryptor> _passwordEncryptorMock;
        private readonly Mock<IJwtProvider> _jwtProviderMock;
        private readonly AuthenticationUseCase _authenticationUseCase;

        public AuthenticationUseCaseTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _validatorMock = new Mock<IValidator<AuthUserDTO>>();
            _passwordEncryptorMock = new Mock<IPasswordEncryptor>();
            _jwtProviderMock = new Mock<IJwtProvider>();

            _authenticationUseCase = new AuthenticationUseCase(
                _userRepositoryMock.Object,
                _validatorMock.Object,
                _passwordEncryptorMock.Object,
                _jwtProviderMock.Object
            );
        }

        [Fact]
        public async Task Handle_ShouldReturnToken_WhenUserIsAuthenticated()
        {
            // Arrange
            var authUserDTO = new AuthUserDTO 
            { 
                Email = "test@example.com", 
                Password = "password123" 
            };

            var request = new AuthUserRequest(authUserDTO); 
            
            var user = new User 
            { 
                Email = authUserDTO.Email, 
                PasswordHash = "hashedPassword" 
            }; 

            var token = new TokenDTO("access_token", "refresh_token");

            _validatorMock.Setup(v => v.ValidateAsync(authUserDTO, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());
            _userRepositoryMock.Setup(repo => repo.FindByEmailAsync(authUserDTO.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);
            _passwordEncryptorMock.Setup(pe => pe.VerifyPassword(user.PasswordHash, authUserDTO.Password))
                .Returns(true);
            _jwtProviderMock.Setup(j => j.GenerateToken(user, true, It.IsAny<CancellationToken>()))
                .ReturnsAsync(token);

            // Act
            var result = await _authenticationUseCase.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(token);
        }

        [Fact]
        public async Task Handle_ShouldThrowValidationException_WhenValidationFails()
        {
            // Arrange
            var authUserDTO = new AuthUserDTO 
            { 
                Email = "", 
                Password = "" 
            }; 

            var request = new AuthUserRequest(authUserDTO);

            var validationErrors = new List<ValidationFailure>
            {
                new ValidationFailure("Email", "Email is required."),
                new ValidationFailure("Password", "Password is required.")
            };

            _validatorMock.Setup(v => v.ValidateAsync(authUserDTO, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult(validationErrors));

            // Act
            var act = async () => await _authenticationUseCase.Handle(request, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ValidationException>();
        }

        [Fact]
        public async Task Handle_ShouldThrowEntityNotFoundException_WhenUserNotFound()
        {
            // Arrange
            var authUserDTO = new AuthUserDTO 
            { 
                Email = "notfound@example.com", 
                Password = "password123" 
            };

            var request = new AuthUserRequest(authUserDTO);

            _validatorMock.Setup(v => v.ValidateAsync(authUserDTO, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());
            _userRepositoryMock.Setup(repo => repo.FindByEmailAsync(authUserDTO.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync((User?)null); 

            // Act
            var act = async () => await _authenticationUseCase.Handle(request, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<EntityNotFoundException>();
        }

        [Fact]
        public async Task Handle_ShouldThrowAuthenticationException_WhenPasswordIsIncorrect()
        {
            // Arrange
            var authUserDTO = new AuthUserDTO 
            { 
                Email = "test@example.com", 
                Password = "wrongpassword" 
            };

            var request = new AuthUserRequest(authUserDTO);

            var user = new User 
            { 
                Email = authUserDTO.Email, 
                PasswordHash = "hashedPassword" 
            };

            _validatorMock.Setup(v => v.ValidateAsync(authUserDTO, It.IsAny<CancellationToken>()))
               .ReturnsAsync(new ValidationResult());
            _userRepositoryMock.Setup(repo => repo.FindByEmailAsync(authUserDTO.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user); 

            _passwordEncryptorMock.Setup(pe => pe.VerifyPassword(user.PasswordHash, authUserDTO.Password))
                .Returns(false); 

            // Act
            var act = async () => await _authenticationUseCase.Handle(request, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<AuthenticationException>();
        }

    }
}

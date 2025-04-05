using AutoMapper;
using Bogus;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using Proto.OrderUser;

namespace UserService
{
    public class CreateUserUseCaseTests
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IValidator<UserDTO>> _validatorMock;
        private readonly CreateUserUseCase _createUserUseCase;
        private readonly Mock<OrderUserService.OrderUserServiceClient> _orderUserServiceMock;

        public CreateUserUseCaseTests()
        {
            _mapperMock = new Mock<IMapper>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _validatorMock = new Mock<IValidator<UserDTO>>();
            _orderUserServiceMock = new Mock<OrderUserService.OrderUserServiceClient>();
            _createUserUseCase = new CreateUserUseCase(_mapperMock.Object, 
                                                       _userRepositoryMock.Object, 
                                                       _validatorMock.Object, 
                                                       _orderUserServiceMock.Object);
        }

        [Fact]
        public async Task Execute_ShouldCreateUser_WhenUserDTOIsValid()
        {
            // Arrange
            var userDTO = new Faker<UserDTO>()
                .RuleFor(u => u.Name, f => f.Name.FullName())
                .RuleFor(u => u.BirthDate, f => f.Date.Past())
                .Generate();

            var userId = Guid.NewGuid();
            var user = new User { Id = userId };

            _validatorMock.Setup(v => v.ValidateAsync(userDTO, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());
            _mapperMock.Setup(m => m.Map<User>(userDTO))
                .Returns(user);
            _userRepositoryMock.Setup(u => u.CreateAsync(user, It.IsAny<CancellationToken>()))
                .ReturnsAsync(userId);

            var mockCall = CallHelpers.CreateAsyncUnaryCall(new Response
            {
                Message = "Test",
                Success = true
            });
            _orderUserServiceMock
                .Setup(m => m.CreateCartAsync(
                    It.IsAny<CartRequest>(), null, null, CancellationToken.None))
                .Returns(mockCall);

            // Act
            var result = await _createUserUseCase.Execute(userDTO, CancellationToken.None);

            // Assert
            result.Should().Be(userId);
            _userRepositoryMock.Verify(u => u.CreateAsync(user, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Execute_ShouldThrowGRPCRequestFailException_WhenGRPCRequestFailed()
        {
            // Arrange
            var userDTO = new Faker<UserDTO>()
                .RuleFor(u => u.Name, f => f.Name.FullName())
                .RuleFor(u => u.BirthDate, f => f.Date.Past())
                .Generate();

            var userId = Guid.NewGuid();
            var user = new User { Id = userId };

            _validatorMock.Setup(v => v.ValidateAsync(userDTO, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());
            _mapperMock.Setup(m => m.Map<User>(userDTO))
                .Returns(user);
            _userRepositoryMock.Setup(u => u.CreateAsync(user, It.IsAny<CancellationToken>()))
                .ReturnsAsync(userId);

            var mockCall = CallHelpers.CreateAsyncUnaryCall(new Response
            {
                Message = "Fail",
                Success = false
            });
            _orderUserServiceMock
                .Setup(m => m.CreateCartAsync(
                    It.IsAny<CartRequest>(), null, null, CancellationToken.None))
                .Returns(mockCall);

            // Act
            var act = async () => await _createUserUseCase.Execute(userDTO, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<GRPCRequestFailException>().WithMessage("Fail");
        }

        [Fact]
        public async Task Execute_ShouldThrowValidationException_WhenUserDTOIsInvalid()
        {
            // Arrange
            var userDTO = new UserDTO(); 
            var errors = new List<ValidationFailure>
            {
                new ValidationFailure("Name", "Name is required"),
                new ValidationFailure("Email", "Email is required")
            };

            _validatorMock.Setup(v => v.ValidateAsync(userDTO, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult(errors));

            // Act
            var act = async () => await _createUserUseCase.Execute(userDTO, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<FluentValidation.ValidationException>();
            _userRepositoryMock.Verify(u => u.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}

using FluentAssertions;
using Moq;
using Proto.AuthUser;

namespace UserService
{
    public class AuthServiceImplTests
    {
        private readonly Mock<ICreateAdminUseCase> _createAdminUseCaseMock;
        private readonly Mock<ICreateUserUseCase> _createUserUseCaseMock;
        private readonly Mock<ICreateManufacturerUseCase> _createManufacturerUseCaseMock;
        private readonly Mock<IDeleteAdminUseCase> _deleteAdminUseCaseMock;
        private readonly Mock<IDeleteUserUseCase> _deleteUserUseCaseMock;
        private readonly Mock<IDeleteManufacturerUseCase> _deleteManufacturerUseCaseMock;
        private readonly AuthServiceImpl _authService;

        public AuthServiceImplTests()
        {
            _createAdminUseCaseMock = new Mock<ICreateAdminUseCase>();
            _createUserUseCaseMock = new Mock<ICreateUserUseCase>();
            _createManufacturerUseCaseMock = new Mock<ICreateManufacturerUseCase>();
            _deleteAdminUseCaseMock = new Mock<IDeleteAdminUseCase>();
            _deleteUserUseCaseMock = new Mock<IDeleteUserUseCase>();
            _deleteManufacturerUseCaseMock = new Mock<IDeleteManufacturerUseCase>();

            _authService = new AuthServiceImpl(
                _createAdminUseCaseMock.Object,
                _createManufacturerUseCaseMock.Object,
                _createUserUseCaseMock.Object,
                _deleteAdminUseCaseMock.Object,
                _deleteManufacturerUseCaseMock.Object,
                _deleteUserUseCaseMock.Object);
        }

        [Fact]
        public async Task CreateAdmin_ShouldReturnSuccess_WhenAdminIsCreated()
        {
            // Arrange
            var request = new CreateAdminRequest
            {
                Id = Guid.NewGuid().ToString(),
                Name = "AdminName",
                Surname = "AdminSurname"
            };
            var cancellationToken = new CancellationToken();
            var expectedId = Guid.NewGuid();

            _createAdminUseCaseMock.Setup(x => x.Execute(It.IsAny<AdminDTO>(), cancellationToken))
                .ReturnsAsync(expectedId);

            // Act
            var response = await _authService.CreateAdmin(request, TestServerCallContext.Create());

            // Assert
            response.Should().NotBeNull();
            response.Success.Should().BeTrue();
            response.Message.Should().Contain(expectedId.ToString());
        }

        [Fact]
        public async Task CreateAdmin_ShouldReturnFailure_WhenExceptionIsThrown()
        {
            // Arrange
            var request = new CreateAdminRequest
            {
                Id = Guid.NewGuid().ToString(),
                Name = "AdminName",
                Surname = "AdminSurname"
            };
            var cancellationToken = new CancellationToken();

            _createAdminUseCaseMock.Setup(x => x.Execute(It.IsAny<AdminDTO>(), cancellationToken))
                .ThrowsAsync(new Exception("Creation failed"));

            // Act
            var response = await _authService.CreateAdmin(request, TestServerCallContext.Create());

            // Assert
            response.Should().NotBeNull();
            response.Success.Should().BeFalse();
            response.Message.Should().Be("Creation failed");
        }

        [Fact]
        public async Task CreateUser_ShouldReturnSuccess_WhenUserIsCreated()
        {
            // Arrange
            var request = new CreateUserRequest
            {
                Id = Guid.NewGuid().ToString(),
                Name = "User Name",
                Surname = "User Surname",
                BirthDate = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(DateTime.UtcNow)
            };
            var cancellationToken = new CancellationToken();
            var expectedId = Guid.NewGuid();

            _createUserUseCaseMock.Setup(x => x.Execute(It.IsAny<UserDTO>(), cancellationToken))
                .ReturnsAsync(expectedId);

            // Act
            var response = await _authService.CreateUser(request, TestServerCallContext.Create());

            // Assert
            response.Should().NotBeNull();
            response.Success.Should().BeTrue();
            response.Message.Should().Contain(expectedId.ToString());
        }

        [Fact]
        public async Task DeleteEntity_ShouldReturnSuccess_WhenEntityIsDeleted()
        {
            // Arrange
            var request = new DeleteEntityRequest
            {
                Id = Guid.NewGuid().ToString(),
                Role = "User"
            };
            var cancellationToken = new CancellationToken();

            _deleteUserUseCaseMock.Setup(x => x.Execute(It.IsAny<Guid>(), cancellationToken))
                .Returns(Task.CompletedTask);

            // Act
            var response = await _authService.DeleteEntity(request, TestServerCallContext.Create());

            // Assert
            response.Should().NotBeNull();
            response.Success.Should().BeTrue();
            response.Message.Should().Contain("deleted successfully");
        }

        [Fact]
        public async Task DeleteEntity_ShouldReturnFailure_WhenExceptionIsThrown()
        {
            // Arrange
            var request = new DeleteEntityRequest
            {
                Id = Guid.NewGuid().ToString(),
                Role = "User"
            };
            var cancellationToken = new CancellationToken();

            _deleteUserUseCaseMock.Setup(x => x.Execute(It.IsAny<Guid>(), cancellationToken))
                .ThrowsAsync(new Exception("Deletion failed"));

            // Act
            var response = await _authService.DeleteEntity(request, TestServerCallContext.Create());

            // Assert
            response.Should().NotBeNull();
            response.Success.Should().BeFalse();
            response.Message.Should().Be("Deletion failed");
        }
    }
}

using FluentAssertions;
using Moq;

namespace UserService
{
    public class DeleteAdminUseCaseTests
    {
        private readonly Mock<IAdminRepository> _adminRepositoryMock;
        private readonly DeleteAdminUseCase _deleteAdminUseCase;

        public DeleteAdminUseCaseTests()
        {
            _adminRepositoryMock = new Mock<IAdminRepository>();
            _deleteAdminUseCase = new DeleteAdminUseCase(_adminRepositoryMock.Object);
        }

        [Fact]
        public async Task Execute_ShouldThrowValidationException_WhenAdminIdIsEmpty()
        {
            // Arrange
            var adminId = Guid.Empty;

            // Act
            Func<Task> act = async () => await _deleteAdminUseCase.Execute(adminId);

            // Assert
            await act.Should().ThrowAsync<FluentValidation.ValidationException>();
        }

        [Fact]
        public async Task Execute_ShouldThrowEntityNotFoundException_WhenAdminDoesNotExist()
        {
            // Arrange
            var adminId = Guid.NewGuid();
            _adminRepositoryMock.Setup(a => a.GetByIdAsync(adminId, CancellationToken.None))
                .ReturnsAsync((Admin?)null); 

            // Act
            Func<Task> act = async () => await _deleteAdminUseCase.Execute(adminId);

            // Assert
            await act.Should().ThrowAsync<EntityNotFoundException>();
        }

        [Fact]
        public async Task Execute_ShouldDeleteAdmin_WhenAdminExists()
        {
            // Arrange
            var adminId = Guid.NewGuid();
            var admin = new Admin { Id = adminId };

            _adminRepositoryMock.Setup(a => a.GetByIdAsync(adminId, CancellationToken.None))
                .ReturnsAsync(admin); 

            // Act
            await _deleteAdminUseCase.Execute(adminId);

            // Assert
            _adminRepositoryMock.Verify(a => a.DeleteAsync(admin, CancellationToken.None), Times.Once);
        }
    }
}

using FluentAssertions;
using Moq;

namespace UserService
{
    public class GetAdminInfoUseCaseTests
    {
        private readonly Mock<IAdminRepository> _adminRepositoryMock;
        private readonly GetAdminInfoUseCase _getAdminInfoUseCase;

        public GetAdminInfoUseCaseTests()
        {
            _adminRepositoryMock = new Mock<IAdminRepository>();
            _getAdminInfoUseCase = new GetAdminInfoUseCase(_adminRepositoryMock.Object);
        }

        [Fact]
        public async Task Execute_ShouldThrowEntityNotFoundException_WhenAdminDoesNotExist()
        {
            // Arrange
            var adminId = Guid.NewGuid();
            _adminRepositoryMock.Setup(a => a.GetByIdAsync(adminId, CancellationToken.None))
                .ReturnsAsync((Admin?)null); 

            // Act
            Func<Task> act = async () => await _getAdminInfoUseCase.Execute(adminId);

            // Assert
            await act.Should().ThrowAsync<EntityNotFoundException>();
        }

        [Fact]
        public async Task Execute_ShouldReturnAdmin_WhenAdminExists()
        {
            // Arrange
            var adminId = Guid.NewGuid();
            var admin = new Admin { Id = adminId, Name = "Test Admin" };

            _adminRepositoryMock.Setup(a => a.GetByIdAsync(adminId, CancellationToken.None))
                .ReturnsAsync(admin); 

            // Act
            var result = await _getAdminInfoUseCase.Execute(adminId);

            // Assert
            result.Should().BeEquivalentTo(admin); 
        }
    }
}

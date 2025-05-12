using Moq;

namespace UserService
{
    public class UpdateUserLogoUseCaseTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly UpdateUserLogoUseCase _useCase;

        public UpdateUserLogoUseCaseTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _useCase = new UpdateUserLogoUseCase(_userRepositoryMock.Object);
        }

        [Fact]
        public async Task Execute_UserExists_UpdatesLogo()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var base64Image = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAUA...";
            var user = new User
            {
                Id = userId,
                Logo = null 
            };

            _userRepositoryMock
                .Setup(repo => repo.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            // Act
            await _useCase.Execute(userId, base64Image, CancellationToken.None);

            // Assert
            Assert.Equal(base64Image, user.Logo);
            _userRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Execute_UserDoesNotExist_ThrowsEntityNotFoundException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var base64Image = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAUA...";

            _userRepositoryMock
                .Setup(repo => repo.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((User?)null); 

            // Act & Assert
            var exception = await Assert.ThrowsAsync<EntityNotFoundException>(() =>
                _useCase.Execute(userId, base64Image, CancellationToken.None));
        }
    }
}

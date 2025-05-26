using Moq;

namespace UserService
{
    public class GetAllAdminsUseCaseTests
    {
        private readonly Mock<IAdminRepository> _adminRepositoryMock;
        private readonly GetAllAdminsUseCase _useCase;

        public GetAllAdminsUseCaseTests()
        {
            _adminRepositoryMock = new Mock<IAdminRepository>();
            _useCase = new GetAllAdminsUseCase(_adminRepositoryMock.Object);
        }

        [Fact]
        public async Task Execute_ReturnsListOfAdmins()
        {
            // Arrange
            var admins = new List<Admin>
            {
                new Admin { Id = Guid.NewGuid(), AdminControlOrdersId = new List<Guid>() },
                new Admin { Id = Guid.NewGuid(), AdminControlOrdersId = new List<Guid>() }
            };

            _adminRepositoryMock
                .Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(admins);

            // Act
            var result = await _useCase.Execute(CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal(admins, result);
        }

        [Fact]
        public async Task Execute_WhenNoAdmins_ReturnsEmptyList()
        {
            // Arrange
            _adminRepositoryMock
                .Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Admin>());

            // Act
            var result = await _useCase.Execute(CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
}

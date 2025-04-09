using FluentAssertions;
using Moq;
using System.Linq.Expressions;

namespace UserService
{
    public class GetUsersWithBirthdayUseCaseTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly GetUsersWithBirthdayUseCase _useCase;

        public GetUsersWithBirthdayUseCaseTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _useCase = new GetUsersWithBirthdayUseCase(_userRepositoryMock.Object);
        }

        [Fact]
        public async Task Execute_ShouldReturnUsersWithBirthday_WhenUsersExist()
        {
            // Arrange
            var today = DateTime.Now.ToUniversalTime();
            var users = new List<User>
            {
                new () { Id = Guid.NewGuid(), BirthDate = today.AddYears(-25) },
                new () { Id = Guid.NewGuid(), BirthDate = today.AddYears(-30) }
            };

            _userRepositoryMock.Setup(repo => repo.GetManyAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(users.Where(x => x.BirthDate.Day == today.Day &&
                                               x.BirthDate.Month == today.Month).ToList());

            // Act
            var result = await _useCase.Execute(CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.Should().Contain(users);
        }

        [Fact]
        public async Task Execute_ShouldReturnEmptyList_WhenNoUsersWithBirthday()
        {
            // Arrange
            var users = new List<User>
            {
                new () { Id = Guid.NewGuid(), BirthDate = DateTime.UtcNow.AddDays(1) }, 
                new () { Id = Guid.NewGuid(), BirthDate = DateTime.UtcNow.AddDays(2) }  
            };

            _userRepositoryMock.Setup(repo => repo.GetManyAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(users.Where(x => x.BirthDate.DayOfYear == DateTime.UtcNow.DayOfYear).ToList());

            // Act
            var result = await _useCase.Execute(CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }
    }
}

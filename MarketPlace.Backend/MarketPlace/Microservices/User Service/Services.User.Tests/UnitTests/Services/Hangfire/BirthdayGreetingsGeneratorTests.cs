using Microsoft.AspNetCore.SignalR;
using Moq;

namespace UserService
{
    public class BirthdayGreetingsGeneratorTests
    {
        private readonly Mock<IGetUsersWithBirthdayUseCase> _getUsersWithBirthdayUseCaseMock;
        private readonly Mock<IAddUserNotificationUseCase> _addUserNotificationUseCaseMock;
        private readonly BirthdayGreetingsGenerator _birthdayGreetingsGenerator;
        private readonly Mock<IHubContext<NotificationHub>> _hubContext;

        public BirthdayGreetingsGeneratorTests()
        {
            _getUsersWithBirthdayUseCaseMock = new Mock<IGetUsersWithBirthdayUseCase>();
            _addUserNotificationUseCaseMock = new Mock<IAddUserNotificationUseCase>();
            _hubContext = new Mock<IHubContext<NotificationHub>>();
            _birthdayGreetingsGenerator = new BirthdayGreetingsGenerator(_getUsersWithBirthdayUseCaseMock.Object, 
                                                                         _addUserNotificationUseCaseMock.Object,
                                                                         _hubContext.Object);
        }

        [Fact]
        public async Task GenerateBirthdayGreetings_ShouldSendGreetings_WhenUsersAreFound()
        {
            // Arrange
            var cancellationToken = new CancellationToken();
            var users = new List<User>
            {
                new User { Id = Guid.NewGuid(), Name = "Alice", BirthDate = DateTime.Now },
                new User { Id = Guid.NewGuid(), Name = "Bob", BirthDate = DateTime.Now }
            };

            _getUsersWithBirthdayUseCaseMock.Setup(x => x.Execute(cancellationToken))
                .ReturnsAsync(users);

            // Act
            await _birthdayGreetingsGenerator.GenerateBirthdayGreetings(cancellationToken);

            // Assert
            _getUsersWithBirthdayUseCaseMock.Verify(x => x.Execute(cancellationToken), Times.Once);
            // TO DO
            // Add greetings logic assertion when it will be implemented
        }

        [Fact]
        public async Task GenerateBirthdayGreetings_ShouldHandleEmptyUserList()
        {
            // Arrange
            var cancellationToken = new CancellationToken();
            var users = new List<User>(); 

            _getUsersWithBirthdayUseCaseMock.Setup(x => x.Execute(cancellationToken))
                .ReturnsAsync(users);

            // Act
            await _birthdayGreetingsGenerator.GenerateBirthdayGreetings(cancellationToken);

            // Assert
            _getUsersWithBirthdayUseCaseMock.Verify(x => x.Execute(cancellationToken), Times.Once);
        }
    }
}

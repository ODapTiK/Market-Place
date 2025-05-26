using Microsoft.AspNetCore.SignalR;

namespace UserService
{
    public class BirthdayGreetingsGenerator
    {
        private readonly IGetUsersWithBirthdayUseCase _getUsersWithBirthdayUseCase;
        private readonly IAddUserNotificationUseCase _addUserNotificationUseCase;
        private readonly IHubContext<NotificationHub> _hubContext;

        public BirthdayGreetingsGenerator(IGetUsersWithBirthdayUseCase getUsersWithBirthdayUseCase, 
                                          IAddUserNotificationUseCase addUserNotificationUseCase,
                                          IHubContext<NotificationHub> hubContext)
        {
            _getUsersWithBirthdayUseCase = getUsersWithBirthdayUseCase;
            _addUserNotificationUseCase = addUserNotificationUseCase;
            _hubContext = hubContext;
        }

        public async Task GenerateBirthdayGreetings(CancellationToken cancellationToken)
        {
            Console.WriteLine("Happy birthday");
            var usersToGreet = await _getUsersWithBirthdayUseCase.Execute(cancellationToken);

            foreach (var user in usersToGreet)
            {
                var notification = new Notification
                {
                    Title = "С Днём Рождения!",
                    Message = $"Команда проекта поздравляет вас с Днём Рождения! Желаем счастья и успехов!",
                    Type = "birthday",
                    IsRead = false,
                    CreatedAt = DateTime.Now.ToUniversalTime()
                };

                await _addUserNotificationUseCase.Execute(user.Id, notification, cancellationToken);

                await _hubContext.Clients.Group(user.Id.ToString()).SendAsync("ReceiveNotification", notification);
            }
        }
    }
}

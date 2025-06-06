namespace UserService
{
    public class ReadUserNotificationUseCase : IReadUserNotificationUseCase
    {
        private readonly IUserRepository _repository;

        public ReadUserNotificationUseCase(IUserRepository repository)
        {
            _repository = repository;
        }

        public async Task Execute(Guid userId, Guid notificationId, CancellationToken cancellationToken)
        {
            var user = await _repository.GetByIdAsync(userId, cancellationToken)
                ?? throw new EntityNotFoundException(nameof(User), userId);

            user.UserNotifications.First(x => x.Id == notificationId).IsRead = true;
            await _repository.UpdateAsync(cancellationToken);
        }
    }
}

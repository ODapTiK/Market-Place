namespace UserService
{
    public interface IReadUserNotificationUseCase
    {
        public Task Execute(Guid userId, Guid notificationId, CancellationToken cancellationToken);
    }
}

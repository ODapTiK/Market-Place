namespace UserService
{
    public interface IAddUserNotificationUseCase
    {
        public Task Execute(Guid userId, Notification notification, CancellationToken cancellationToken);
    }
}

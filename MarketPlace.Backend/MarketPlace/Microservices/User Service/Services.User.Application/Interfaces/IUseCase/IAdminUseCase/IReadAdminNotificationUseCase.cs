namespace UserService
{
    public interface IReadAdminNotificationUseCase
    {
        public Task Execute(Guid adminId, Guid notificationId, CancellationToken cancellationToken);
    }
}

namespace UserService
{
    public interface IAddAdminNotificationUseCase
    {
        public Task Execute(Guid adminId, Notification notification, CancellationToken cancellationToken);
    }
}

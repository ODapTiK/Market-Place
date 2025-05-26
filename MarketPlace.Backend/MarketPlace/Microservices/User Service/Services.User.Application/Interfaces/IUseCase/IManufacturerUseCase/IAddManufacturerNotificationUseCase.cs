namespace UserService
{
    public interface IAddManufacturerNotificationUseCase
    {
        public Task Execute(Guid manufacturerId, Notification notification, CancellationToken cancellationToken);
    }
}

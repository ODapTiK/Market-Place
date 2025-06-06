namespace UserService
{
    public interface IReadManufacturerNotificationUseCase
    {
        public Task Execute(Guid manufacturerId, Guid notificationId, CancellationToken cancellationToken);
    }
}

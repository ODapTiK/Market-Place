namespace UserService
{
    public class ReadManufacturerNotificationUseCase : IReadManufacturerNotificationUseCase
    {
        private readonly IManufacturerRepository _manufacturerRepository;

        public ReadManufacturerNotificationUseCase(IManufacturerRepository manufacturerRepository)
        {
            _manufacturerRepository = manufacturerRepository;
        }

        public async Task Execute(Guid manufacturerId, Guid notificationId, CancellationToken cancellationToken)
        {
            var manufacturer = await _manufacturerRepository.GetByIdAsync(manufacturerId, cancellationToken)
               ?? throw new EntityNotFoundException(nameof(Manufacturer), manufacturerId);

            manufacturer.ManufacturerNotifications.First(x => x.Id == notificationId).IsRead = true;
            await _manufacturerRepository.UpdateAsync(cancellationToken);
        }
    }
}

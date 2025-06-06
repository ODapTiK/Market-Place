namespace UserService
{
    public class AddManufacturerNotificationUseCase : IAddManufacturerNotificationUseCase
    {
        private readonly IManufacturerRepository _repository;

        public AddManufacturerNotificationUseCase(IManufacturerRepository repository)
        {
            _repository = repository;
        }

        public async Task Execute(Guid manufacturerId, Notification notification, CancellationToken cancellationToken)
        {
            var user = await _repository.GetByIdAsync(manufacturerId, cancellationToken)
                ?? throw new EntityNotFoundException(nameof(Manufacturer), manufacturerId);

            await _repository.AddNotificationAsync(user, notification, cancellationToken);
        }
    }
}

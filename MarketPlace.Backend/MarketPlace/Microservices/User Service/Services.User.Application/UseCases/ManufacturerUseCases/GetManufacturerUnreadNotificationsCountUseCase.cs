namespace UserService
{
    public class GetManufacturerUnreadNotificationsCountUseCase : IGetManufacturerUnreadNotificationsCountUseCase
    {
        private readonly IManufacturerRepository _manufacturerRepository;

        public GetManufacturerUnreadNotificationsCountUseCase(IManufacturerRepository manufacturerRepository)
        {
            _manufacturerRepository = manufacturerRepository;
        }

        public async Task<int> Execute(Guid manufacturerId, CancellationToken cancellationToken)
        {
            var manufacturer = await _manufacturerRepository.GetByIdAsync(manufacturerId, cancellationToken)
                ?? throw new EntityNotFoundException(nameof(Manufacturer), manufacturerId);

            return manufacturer.ManufacturerNotifications.Where(x => x.IsRead == false).ToList().Count();
        }
    }
}

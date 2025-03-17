namespace UserService
{
    public class DeleteManufacturerUseCase : IDeleteManufacturerUseCase
    {
        private readonly IManufacturerRepository _manufacturerRepository;

        public DeleteManufacturerUseCase(IManufacturerRepository manufacturerRepository)
        {
            _manufacturerRepository = manufacturerRepository;
        }

        public async Task Execute(Guid manufacturerId)
        {
            if (manufacturerId == Guid.Empty)
                throw new FluentValidation.ValidationException("Manufacturer Id must not be empty!");

            var manufacturer = await _manufacturerRepository.GetByIdAsync(manufacturerId, CancellationToken.None);
            if (manufacturer == null)
                throw new EntityNotFoundException(nameof(Manufacturer), manufacturerId);

            await _manufacturerRepository.DeleteAsync(manufacturer, CancellationToken.None);
        }
    }
}

namespace UserService
{
    public class AddManufacturerProductUseCase : IAddManufacturerProductUseCase
    {
        private readonly IManufacturerRepository _manufacturerRepository;

        public AddManufacturerProductUseCase(IManufacturerRepository manufacturerRepository)
        {
            _manufacturerRepository = manufacturerRepository;
        }

        public async Task Execute(Guid manufacturerId, Guid productId)
        {
            if (manufacturerId == Guid.Empty)
                throw new FluentValidation.ValidationException("Manufacturer Id must not be empty");
            else if (productId == Guid.Empty)
                throw new FluentValidation.ValidationException("Product Id must not be empty");

            var manufacturer = await _manufacturerRepository.GetByIdAsync(manufacturerId, CancellationToken.None);
            if (manufacturer == null)
                throw new EntityNotFoundException(nameof(Manufacturer), manufacturerId);

            await _manufacturerRepository.AddProductAsync(manufacturer, productId, CancellationToken.None);
        }
    }
}

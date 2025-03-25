namespace UserService
{
    public class GetManufacturerInfoUseCase : IGetManufacturerInfoUseCase
    {
        private readonly IManufacturerRepository _manufacturerRepository;

        public GetManufacturerInfoUseCase(IManufacturerRepository manufacturerRepository)
        {
            _manufacturerRepository = manufacturerRepository;
        }

        public async Task<Manufacturer> Execute(Guid manufacturerId, CancellationToken cancellationToken)
        {
            var manufacturer = await _manufacturerRepository.GetByIdAsync(manufacturerId, cancellationToken);
            if (manufacturer == null) 
                throw new EntityNotFoundException(nameof(Manufacturer), manufacturerId);

            return manufacturer;
        }
    }
}

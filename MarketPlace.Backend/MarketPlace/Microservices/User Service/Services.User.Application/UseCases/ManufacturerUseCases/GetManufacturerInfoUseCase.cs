namespace UserService
{
    public class GetManufacturerInfoUseCase : IGetManufacturerInfoUseCase
    {
        private readonly IManufacturerRepository _manufacturerRepository;

        public GetManufacturerInfoUseCase(IManufacturerRepository manufacturerRepository)
        {
            _manufacturerRepository = manufacturerRepository;
        }

        public async Task<Manufacturer> Execute(Guid manufacturerId)
        {
            var manufacturer = await _manufacturerRepository.GetByIdAsync(manufacturerId, CancellationToken.None);
            if (manufacturer == null) 
                throw new EntityNotFoundException(nameof(Manufacturer), manufacturerId);

            return manufacturer;
        }
    }
}

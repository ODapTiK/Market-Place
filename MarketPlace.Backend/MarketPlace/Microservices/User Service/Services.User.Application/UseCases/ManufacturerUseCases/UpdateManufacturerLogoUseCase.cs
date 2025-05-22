
namespace UserService
{
    public class UpdateManufacturerLogoUseCase : IUpdateManufacturerLogoUseCase
    {
        private readonly IManufacturerRepository _manufacturerRepository;

        public UpdateManufacturerLogoUseCase(IManufacturerRepository manufacturerRepository)
        {
            _manufacturerRepository = manufacturerRepository;
        }

        public async Task Execute(Guid manufacturerId, string base64Image, CancellationToken cancellationToken)
        {
            var manufacturer = await _manufacturerRepository.GetByIdAsync(manufacturerId, cancellationToken)
                ?? throw new EntityNotFoundException(nameof(Manufacturer), manufacturerId);

            manufacturer.Logo = base64Image;

            await _manufacturerRepository.UpdateAsync(cancellationToken);
        }
    }
}

namespace UserService
{
    public interface IUpdateManufacturerLogoUseCase
    {
        public Task Execute(Guid manufacturerId, string base64Image, CancellationToken cancellationToken);
    }
}

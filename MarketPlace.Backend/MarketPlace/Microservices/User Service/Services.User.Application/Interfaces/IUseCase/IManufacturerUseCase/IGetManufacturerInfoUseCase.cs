namespace UserService
{
    public interface IGetManufacturerInfoUseCase
    {
        public Task<Manufacturer> Execute(Guid manufacturerId);
    }
}

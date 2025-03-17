namespace UserService
{
    public interface IDeleteManufacturerUseCase
    {
        public Task Execute(Guid manufacturerId);
    }
}

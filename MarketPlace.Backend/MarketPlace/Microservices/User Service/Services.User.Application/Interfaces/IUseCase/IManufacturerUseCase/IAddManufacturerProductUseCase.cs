namespace UserService
{
    public interface IAddManufacturerProductUseCase
    {
        public Task Execute(Guid manufacturerId, Guid productId);
    }
}

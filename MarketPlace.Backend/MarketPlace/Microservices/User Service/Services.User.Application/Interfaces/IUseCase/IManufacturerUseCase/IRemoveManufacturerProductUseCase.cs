namespace UserService
{
    public interface IRemoveManufacturerProductUseCase
    {
        public Task Execute(Guid manufacturerId, Guid productId);
    }
}

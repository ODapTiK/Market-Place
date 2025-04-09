namespace UserService
{
    public interface IManufacturerRepository : IBaseRepository<Manufacturer>
    {
        public Task AddProductAsync(Manufacturer manufacturer, Guid productId, CancellationToken cancellationToken);
        public Task RemoveProductAsync(Manufacturer manufactorer, Guid productId, CancellationToken cancellationToken);
        public Task<List<Manufacturer>> GetAllAsync(CancellationToken cancellationToken);
    }
}

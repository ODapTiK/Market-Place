namespace OrderService
{
    public interface ICartRepository : IBaseRepository<Cart>
    {
        public Task<Cart?> GetUserCartAsync(Guid userId, CancellationToken cancellationToken);
        public Task AddOrderPointAsync(Cart cart, Guid orderPoinproductId, CancellationToken cancellationToken);
        public Task RemoveOrderPointAsync(Cart cart, Guid productId, CancellationToken cancellationToken);
    }
}

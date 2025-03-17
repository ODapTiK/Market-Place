namespace OrderService
{
    public interface IOrderRepository : IBaseRepository<Order>
    {
        public Task<List<Order>> GetUserOrdersAsync(Guid userId, CancellationToken cancellationToken);
    }
}

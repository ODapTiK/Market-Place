using System.Linq.Expressions;

namespace OrderService
{
    public interface IOrderRepository : IBaseRepository<Order>
    {
        public Task<List<Order>> GetUserOrdersAsync(Guid userId, CancellationToken cancellationToken);
        public Task<List<Order>> GetManyOrdersAsync(Expression<Func<Order, bool>> predicate, CancellationToken cancellationToken);
        public Task DeleteUserOrdersAsync(Guid userId, CancellationToken cancellationToken);
    }
}

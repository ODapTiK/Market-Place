using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace OrderService
{
    public class OrderRepository : BaseRepository<Order>, IOrderRepository
    {
        public OrderRepository(IOrderDbContext orderDbContext) : base(orderDbContext) { }

        public async Task<List<Order>> GetUserOrdersAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await _context.Orders.Include(x => x.OrderPoints).Where(x => x.UserId.Equals(userId)).ToListAsync();
        }

        public async Task<List<Order>> GetManyOrdersAsync(Expression<Func<Order, bool>> predicate, CancellationToken cancellationToken)
        {
            return await _context.Orders.Include(x => x.OrderPoints).Where(predicate).ToListAsync();
        }

        public async Task DeleteUserOrdersAsync(Guid userId, CancellationToken cancellationToken)
        {
            var ordersToDelete = await _context.Orders.Where(x => x.UserId.Equals(userId)).ToListAsync(cancellationToken);
            _context.Orders.RemoveRange(ordersToDelete);

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}

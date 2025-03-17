
using Microsoft.EntityFrameworkCore;

namespace OrderService
{
    public class OrderRepository : BaseRepository<Order>, IOrderRepository
    {
        public OrderRepository(IOrderDbContext orderDbContext) : base(orderDbContext) { }

        public async Task<List<Order>> GetUserOrdersAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await _context.Orders.Include(x => x.OrderPoints).Where(x => x.UserId.Equals(userId)).ToListAsync();
        }
    }
}

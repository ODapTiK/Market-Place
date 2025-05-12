using Microsoft.EntityFrameworkCore;

namespace OrderService
{
    public class CartRepository : BaseRepository<Cart>, ICartRepository
    {
        public CartRepository(IOrderDbContext context) : base(context) { }

        public async Task AddOrderPointAsync(Cart cart, Guid orderPoinproductId, CancellationToken cancellationToken)
        {
            cart.Products.Add(orderPoinproductId);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<Cart?> GetUserCartAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await _context.Carts.FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);
        }

        public async Task RemoveOrderPointAsync(Cart cart, Guid productId, CancellationToken cancellationToken)
        {
            cart.Products.Remove(productId);
            await _context.SaveChangesAsync(cancellationToken); 
        }

        public async Task RemoveProductFromCartsAsync(Guid productId, CancellationToken cancellationToken)
        {
            var carts = await _context.Carts.Where(x => x.Products.Contains(productId)).ToListAsync(cancellationToken);
            foreach (var cart in carts)
            {
                cart.Products.Remove(productId);
            }

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}


namespace UserService
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(IUserDbContext context) : base(context) { }

        public async Task AddOrderAsync(User user, Guid orderId, CancellationToken cancellationToken)
        {
            user.UserOrdersId.Add(orderId);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task RemoveOrderAsync(User user, Guid orderId, CancellationToken cancellationToken)
        {
            user.UserOrdersId.Remove(orderId);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}

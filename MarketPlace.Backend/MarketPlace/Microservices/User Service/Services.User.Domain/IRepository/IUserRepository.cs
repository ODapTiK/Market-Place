using System.Linq.Expressions;

namespace UserService
{
    public interface IUserRepository : IBaseRepository<User>
    {
        public Task AddOrderAsync(User user, Guid orderId, CancellationToken cancellationToken);
        public Task RemoveOrderAsync(User user, Guid orderId, CancellationToken cancellationToken);
        public Task<List<User>> GetManyAsync(Expression<Func<User, bool>> predicate, CancellationToken cancellationToken);
    }
}

namespace UserService
{
    public interface IUserRepository : IBaseRepository<User>
    {
        public Task AddOrderAsync(User user, Guid orderId, CancellationToken cancellationToken);
        public Task RemoveOrderAsync(User user, Guid orderId, CancellationToken cancellationToken);
    }
}

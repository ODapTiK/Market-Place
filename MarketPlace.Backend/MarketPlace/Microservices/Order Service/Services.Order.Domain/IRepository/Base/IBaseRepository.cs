namespace OrderService
{
    public interface IBaseRepository<T> where T : class
    {
        public Task<Guid> CreateAsync(T entity, CancellationToken cancellationToken);
        public Task UpdateAsync(CancellationToken cancellationToken);
        public Task DeleteAsync(T entity, CancellationToken cancellationToken);
        public Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    }
}

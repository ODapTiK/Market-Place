namespace ProductService
{
    public interface IBaseRepository<T> where T : class
    {
        public Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        public Task<Guid> CreateAsync(T entity, CancellationToken cancellationToken);
        public Task DeleteAsync(T entity, CancellationToken cancellationToken);
        public Task UpdateAsync(T entity, CancellationToken cancellationToken);
    }
}

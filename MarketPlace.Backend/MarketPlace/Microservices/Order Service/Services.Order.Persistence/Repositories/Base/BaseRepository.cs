using Microsoft.EntityFrameworkCore;

namespace OrderService
{
    public abstract class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        protected readonly IOrderDbContext _context;
        private readonly DbSet<T> _dbSet;

        public BaseRepository(IOrderDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<Guid> CreateAsync(T entity, CancellationToken cancellationToken)
        {
            _dbSet.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);
            return (Guid)typeof(T).GetProperty("Id")?.GetValue(entity);
        }

        public async Task DeleteAsync(T entity, CancellationToken cancellationToken)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _dbSet.FindAsync([id], cancellationToken);
        }

        public async Task UpdateAsync(CancellationToken cancellationToken)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}

using Microsoft.EntityFrameworkCore;

namespace UserService
{
    public abstract class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        protected readonly IUserDbContext _context;
        private readonly DbSet<T> _dbSet;

        public BaseRepository(IUserDbContext userDbContext)
        {
            _context = userDbContext;
            _dbSet = userDbContext.Set<T>();
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


using MongoDB.Driver;

namespace ProductService
{
    public abstract class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        protected readonly IMongoCollection<T> _collection;

        public BaseRepository(IProductDbContext context, string collectionName)
        {
            _collection = context.GetCollection<T>(collectionName);
        }
        public async Task<Guid> CreateAsync(T entity, CancellationToken cancellationToken)
        {
            var options = new InsertOneOptions();

            await _collection.InsertOneAsync(entity, options, cancellationToken);

            return (Guid)typeof(T).GetProperty("Id")?.GetValue(entity);
        }

        public async Task DeleteAsync(T entity, CancellationToken cancellationToken)
        {
            await _collection.DeleteOneAsync(Builders<T>.Filter.Eq("Id", (Guid)typeof(T).GetProperty("Id")?.GetValue(entity)), cancellationToken);
        }

        public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var filter = Builders<T>.Filter.Where(item => ((Guid)item.GetType().GetProperty("Id").GetValue(item)) == id);
            var options = new FindOptions<T, T>();

            using (var cursor = await _collection.FindAsync(filter, options, cancellationToken))
            {
                var results = await cursor.ToListAsync(cancellationToken);
                
                return results.FirstOrDefault(); 
            }
        }

        public async Task UpdateAsync(T entity, CancellationToken cancellationToken)
        {
            await _collection.ReplaceOneAsync(Builders<T>.Filter.Eq("Id", (Guid)typeof(T).GetProperty("Id")?.GetValue(entity)), entity);
        }
    }
}

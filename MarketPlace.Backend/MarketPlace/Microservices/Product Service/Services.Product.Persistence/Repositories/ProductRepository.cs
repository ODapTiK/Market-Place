using MongoDB.Driver;
using System.Linq.Expressions;

namespace ProductService
{
    public class ProductRepository : BaseRepository<Product>, IProductRepository
    {
        public ProductRepository(IProductDbContext context) : base(context, "Products") { }

        public async Task<List<Product>> GetAllAsync(CancellationToken cancellationToken)
        {
            var filter = Builders<Product>.Filter.Where(_ => true);
            var options = new FindOptions<Product, Product>();

            using (var cursor = await _collection.FindAsync(filter, options, cancellationToken))
            {
                return await cursor.ToListAsync(cancellationToken);
            }
        }

        public async Task<List<Product>> GetManyProductsAsync(Expression<Func<Product, bool>> predicate, CancellationToken cancellationToken)
        {
            var filter = Builders<Product>.Filter.Where(predicate);
            var options = new FindOptions<Product, Product>();

            using (var cursor = await _collection.FindAsync(filter, options, cancellationToken))
            {
                return await cursor.ToListAsync(cancellationToken);
            }
        }
    }
}

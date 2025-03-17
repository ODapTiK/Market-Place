using System.Linq.Expressions;

namespace ProductService
{
    public interface IProductRepository : IBaseRepository<Product>
    {
        public Task<List<Product>> GetAllAsync(CancellationToken cancellationToken);
        public Task<List<Product>> GetManyProductsAsync(Expression<Func<Product, bool>> predicate, CancellationToken cancellationToken);
    }
}

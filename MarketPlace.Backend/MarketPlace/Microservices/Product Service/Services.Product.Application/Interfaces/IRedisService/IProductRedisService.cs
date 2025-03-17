using System.Linq.Expressions;

namespace ProductService
{
    public interface IProductRedisService : IBaseRedisService<Product>
    {
        public Task<List<Product>> GetManyCachedProducts(Func<Product, bool> predicate);
    }
}

using StackExchange.Redis;

namespace ProductService
{
    public class ProductRedisService : BaseRedisService<Product>, IProductRedisService
    {
        public ProductRedisService(IConnectionMultiplexer connectionMultiplexer) : base(connectionMultiplexer, 0) { }
        public async Task<List<Product>> GetManyCachedProducts(Func<Product, bool> predicate)
        {
            var allValues = await GetAllCacheValues();
            return allValues.Where(predicate).ToList();
        }
    }
}

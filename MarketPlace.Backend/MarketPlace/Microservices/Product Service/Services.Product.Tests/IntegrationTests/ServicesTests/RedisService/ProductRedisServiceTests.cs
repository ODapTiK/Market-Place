using StackExchange.Redis;
using FluentAssertions;

namespace ProductService
{
    public class ProductRedisServiceTests
    {
        private readonly ConnectionMultiplexer _connectionMultiplexer;
        private readonly ProductRedisService _productRedisService;
        private const string RedisConnectionString = "localhost:6379";
        private bool _disposed;

        public ProductRedisServiceTests()
        {
            if(_disposed)
                throw new ObjectDisposedException(ToString());

            _connectionMultiplexer = ConnectionMultiplexer.Connect(RedisConnectionString);
            _productRedisService = new ProductRedisService(_connectionMultiplexer);
        }

        [Fact]
        public async Task GetManyCachedProducts_ShouldReturnFilteredProducts()
        {
            // Arrange
            var product1 = new Product { Id = Guid.NewGuid(), Name = "Product A", Price = 100 };
            var product2 = new Product { Id = Guid.NewGuid(), Name = "Product B", Price = 200 };
            var product3 = new Product { Id = Guid.NewGuid(), Name = "Product C", Price = 150 };

            await _productRedisService.SetCacheValue($"product:{product1.Id}", product1);
            await _productRedisService.SetCacheValue($"product:{product2.Id}", product2);
            await _productRedisService.SetCacheValue($"product:{product3.Id}", product3);

            // Act
            var filteredProducts = await _productRedisService.GetManyCachedProducts(p => p.Price > 100);

            // Assert
            filteredProducts.Should().ContainSingle(p => p.Id == product2.Id);
            filteredProducts.Should().ContainSingle(p => p.Id == product3.Id);
            filteredProducts.Should().NotContain(p => p.Id == product1.Id);
        }

        [Fact]
        public async Task GetManyCachedProducts_ShouldReturnEmptyList_WhenNoProductsMatch()
        {
            // Arrange
            var product1 = new Product { Id = Guid.NewGuid(), Name = "Product A", Price = 100 };
            await _productRedisService.SetCacheValue($"product:{product1.Id}", product1);

            // Act
            var filteredProducts = await _productRedisService.GetManyCachedProducts(p => p.Price > 200);

            // Assert
            filteredProducts.Should().BeEmpty();
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                var server = _connectionMultiplexer.GetServer(_connectionMultiplexer.Configuration);
                var keys = server.Keys(database: 0);
                var database = _connectionMultiplexer.GetDatabase(0);
                foreach (var key in keys)
                {
                    database.KeyDelete(key);
                }
                _connectionMultiplexer.Dispose();
            }
        }
    }
}

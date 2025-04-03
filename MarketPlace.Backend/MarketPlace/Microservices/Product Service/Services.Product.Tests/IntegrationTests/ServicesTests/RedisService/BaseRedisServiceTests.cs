using FluentAssertions;
using StackExchange.Redis;

namespace ProductService
{
    public class BaseRedisServiceTests : IDisposable
    {
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        private readonly SampleRedisService _redisService;
        private const string RedisConnectionString = "localhost:6379"; 
        private const int DatabaseIndex = 10;

        public BaseRedisServiceTests()
        {
            _connectionMultiplexer = ConnectionMultiplexer.Connect(RedisConnectionString);
            _redisService = new SampleRedisService(_connectionMultiplexer, DatabaseIndex);
        }

        [Fact]
        public async Task SetCacheValue_ShouldStoreValueInRedis()
        {
            // Arrange
            var key = "sample:1";
            var entity = new SampleEntity { Name = "John", Age = 30 };

            // Act
            await _redisService.SetCacheValue(key, entity);

            // Assert
            var storedEntity = await _redisService.GetCacheValue(key);
            storedEntity.Should().BeEquivalentTo(entity);
        }

        [Fact]
        public async Task GetCacheValue_ShouldThrowEntityNotFoundException_WhenKeyDoesNotExist()
        {
            // Arrange
            var key = "nonexistent:key";

            // Act & Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(async () => await _redisService.GetCacheValue(key));
        }

        [Fact]
        public async Task RemoveCacheValue_ShouldDeleteValueFromRedis()
        {
            // Arrange
            var key = "sample:2";
            var entity = new SampleEntity { Name = "Jane", Age = 25 };
            await _redisService.SetCacheValue(key, entity);

            // Act
            await _redisService.RemoveCacheValue(key);

            // Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(async () => await _redisService.GetCacheValue(key));
        }

        [Fact]
        public async Task GetAllCacheValues_ShouldReturnAllStoredEntities()
        {
            // Arrange
            var key1 = "sample:3";
            var entity1 = new SampleEntity { Name = "Alice", Age = 28 };
            await _redisService.SetCacheValue(key1, entity1);

            var key2 = "sample:4";
            var entity2 = new SampleEntity { Name = "Bob", Age = 32 };
            await _redisService.SetCacheValue(key2, entity2);

            // Act
            var allEntities = await _redisService.GetAllCacheValues();

            // Assert
            allEntities.Should().Contain(e => e.Name == entity1.Name && e.Age == entity1.Age);
            allEntities.Should().Contain(e => e.Name == entity2.Name && e.Age == entity2.Age);
        }

        public void Dispose()
        {
            var server = _connectionMultiplexer.GetServer(RedisConnectionString);
            var keys = server.Keys(database: DatabaseIndex);
            var database = _connectionMultiplexer.GetDatabase(DatabaseIndex);
            foreach (var key in keys)
            {
                database.KeyDelete(key);
            }
            _connectionMultiplexer.Dispose();
        }
    }
}

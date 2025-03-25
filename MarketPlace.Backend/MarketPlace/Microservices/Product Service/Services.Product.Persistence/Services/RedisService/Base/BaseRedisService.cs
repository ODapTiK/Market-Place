using Newtonsoft.Json;
using StackExchange.Redis;
using System.Reflection;

namespace ProductService
{
    public class BaseRedisService<T> : IBaseRedisService<T> where T : class
    {
        protected readonly IDatabase _redisDatabase;

        public BaseRedisService(IConnectionMultiplexer connectionMultiplexer, int databaseIndex) // databaseIndex - индекс БД в Redis, поддерживает 16 БД (0-15)
        {
            if (!(databaseIndex >= 0 && databaseIndex < 16))
                throw new ArgumentException("Database index must be in range (0-15)!");

            _redisDatabase = connectionMultiplexer.GetDatabase(databaseIndex);
        }

        public async Task SetCacheValue(string key, T value)
        {
            var hashEntries = CreateHashEntries(value);
            await _redisDatabase.HashSetAsync(key, hashEntries);
        }

        public async Task<T> GetCacheValue(string key)
        {
            if (await _redisDatabase.KeyExistsAsync(key))
            {
                var hashEntries = await _redisDatabase.HashGetAllAsync(key);
                return CreateEntity(hashEntries);
            }
            else
            {
                throw new EntityNotFoundException(typeof(T).Name, key);
            }
        }

        public async Task RemoveCacheValue(string key)
        {
            if (await _redisDatabase.KeyExistsAsync(key))
            {
                await _redisDatabase.KeyDeleteAsync(key);
            }
            else
            {
                throw new EntityNotFoundException(typeof(T).Name, key);
            }
        }

        public async Task<List<T>> GetAllCacheValues()
        {
            var server = _redisDatabase.Multiplexer.GetServer(_redisDatabase.Multiplexer.Configuration);
            var keys = server.Keys(database: _redisDatabase.Database).ToArray();
            var tasks = keys.Select(key => GetCacheValue(key)).ToArray();
            var entities = await Task.WhenAll(tasks);
            return entities.ToList();
        }

        public bool IsConnected() => _redisDatabase.Multiplexer.IsConnected;

        // Метод для создания HashEntry из объекта
        protected virtual HashEntry[] CreateHashEntries(T value)
        {
            PropertyInfo[] properties = value.GetType().GetProperties();
            return properties
                .Where(x => x.GetValue(value) != null) 
                .Select
                (
                      property =>
                      {
                          object propertyValue = property.GetValue(value);
                          string hashValue;

                          if (propertyValue is IEnumerable<object>)
                          {
                              hashValue = JsonConvert.SerializeObject(propertyValue);
                          }
                          else
                          {
                              hashValue = propertyValue.ToString();
                          }

                          return new HashEntry(property.Name, hashValue);
                      }
                )
                .ToArray();
        }

        // Метод для создания объекта T из HashEntry
        protected virtual T CreateEntity(HashEntry[] hashEntries)
        {
            PropertyInfo[] properties = typeof(T).GetProperties();
            var value = Activator.CreateInstance(typeof(T));
            foreach (var property in properties)
            {
                HashEntry entry = hashEntries.FirstOrDefault(g => g.Name.ToString().Equals(property.Name));
                if (entry.Equals(new HashEntry())) continue;
                property.SetValue(value, Convert.ChangeType(entry.Value.ToString(), property.PropertyType));
            }
            return (T)value;
        }
    }
}

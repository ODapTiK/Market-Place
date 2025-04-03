using Newtonsoft.Json;
using StackExchange.Redis;
using System.Reflection;

namespace ProductService
{
    public abstract class BaseRedisService<T> : IBaseRedisService<T> where T : class
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
            var jsonSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented, 
                NullValueHandling = NullValueHandling.Ignore, 
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore, 
            };

            PropertyInfo[] properties = value.GetType().GetProperties();
            return properties
                .Where(x => x.GetValue(value) != null) 
                .Select
                (
                      property =>
                      {
                          object propertyValue = property.GetValue(value);
                          string hashValue;

                          if (propertyValue is IEnumerable<object> collection)
                          {
                              hashValue = JsonConvert.SerializeObject(collection, jsonSettings);
                          }
                          else if(propertyValue.GetType() == typeof(Guid) || propertyValue.GetType() == typeof(DateTime))
                          {
                              hashValue = propertyValue.ToString();
                          }
                          else if (propertyValue != null && !propertyValue.GetType().IsPrimitive && propertyValue.GetType() != typeof(string))
                          {
                              hashValue = JsonConvert.SerializeObject(propertyValue, jsonSettings);
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
            var jsonSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            };
            PropertyInfo[] properties = typeof(T).GetProperties();
            var value = Activator.CreateInstance(typeof(T));
            foreach (var property in properties)
            {
                HashEntry entry = hashEntries.FirstOrDefault(g => g.Name.ToString().Equals(property.Name));
                if (entry.Equals(new HashEntry())) continue;
                object propertyValue;
                if (property.PropertyType == typeof(Guid))
                {
                    propertyValue = Guid.Parse(entry.Value);
                }
                else if (property.PropertyType == typeof(DateTime))
                {
                    propertyValue = DateTime.Parse(entry.Value);
                }
                else if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    var underlyingType = Nullable.GetUnderlyingType(property.PropertyType);
                    propertyValue = string.IsNullOrEmpty(entry.Value) ? null : Convert.ChangeType(entry.Value, underlyingType);
                }
                else if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    var itemType = property.PropertyType.GenericTypeArguments[0];
                    var json = entry.Value;

                    if (string.IsNullOrWhiteSpace(json))
                    {
                        propertyValue = Activator.CreateInstance(property.PropertyType); // Создаем пустой список
                    }
                    else
                    {
                        propertyValue = JsonConvert.DeserializeObject(json, property.PropertyType, jsonSettings);
                    }
                }
                else if (!property.PropertyType.IsPrimitive && property.PropertyType != typeof(string))
                {
                    propertyValue = JsonConvert.DeserializeObject(entry.Value, property.PropertyType, jsonSettings);
                }
                else
                {
                    propertyValue = Convert.ChangeType(entry.Value, property.PropertyType);
                }
                property.SetValue(value, propertyValue);
            }
            return (T)value;
        }
    }
}

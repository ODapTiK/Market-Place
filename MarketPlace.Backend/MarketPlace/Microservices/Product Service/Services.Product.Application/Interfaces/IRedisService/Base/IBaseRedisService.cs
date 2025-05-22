namespace ProductService
{
    public interface IBaseRedisService<T> where T : class
    {
        Task SetCacheValue(string key, T value);
        Task<T?> GetCacheValue(string key);
        Task RemoveCacheValue(string key);
        Task<List<T>> GetAllCacheValues();
        bool IsConnected();
    }
}

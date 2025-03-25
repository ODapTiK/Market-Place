using MongoDB.Driver;

namespace ProductService
{
    public interface IProductDbContext
    {
        IMongoCollection<T> GetCollection<T>(string collectionName);
    }
}

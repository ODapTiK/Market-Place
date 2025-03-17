using MongoDB.Driver;

namespace ProductService
{
    public class ProductDbContext : IProductDbContext
    {
        private readonly IMongoDatabase _database;

        public ProductDbContext(IMongoClient client, string databaseName)
        {
            _database = client.GetDatabase(databaseName);
        }

        public IMongoCollection<T> GetCollection<T>(string collectionName)
        {
            return _database.GetCollection<T>(collectionName);
        }
    }
}

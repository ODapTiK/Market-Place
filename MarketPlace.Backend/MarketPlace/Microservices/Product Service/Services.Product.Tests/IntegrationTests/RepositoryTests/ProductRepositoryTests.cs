using FluentAssertions;
using MongoDB.Driver;

namespace ProductService
{
    [Collection("Global Collection")]
    public class ProductRepositoryTests : IDisposable
    {
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<Product> _collection;
        private readonly ProductRepository _repository;

        public ProductRepositoryTests()
        {
            var client = new MongoClient("mongodb://localhost:27017");
            _database = client.GetDatabase("TestDatabase");
            _collection = _database.GetCollection<Product>("Products");
            _repository = new ProductRepository(new ProductDbContext(client, "TestDatabase"));
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllProducts()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product { Id = Guid.NewGuid(), Name = "Product 1" },
                new Product { Id = Guid.NewGuid(), Name = "Product 2" }
            };

            await _collection.InsertManyAsync(products);

            // Act
            var result = await _repository.GetAllAsync(CancellationToken.None);

            // Assert
            result.Should().HaveCount(products.Count);
            result.Should().BeEquivalentTo(products, options => options.WithStrictOrdering());
        }

        [Fact]
        public async Task GetManyProductsAsync_ShouldReturnFilteredProducts()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product { Id = Guid.NewGuid(), Name = "Apple" },
                new Product { Id = Guid.NewGuid(), Name = "Banana" },
                new Product { Id = Guid.NewGuid(), Name = "Cherry" }
            };

            await _collection.InsertManyAsync(products);

            // Act
            var result = await _repository.GetManyProductsAsync(p => p.Name.StartsWith("A"), CancellationToken.None);

            // Assert
            result.Should().HaveCount(1);
            result.Should().ContainSingle(p => p.Name == "Apple");
        }

        public void Dispose()
        {
            // Очистка коллекции для тестов
            _database.DropCollection("Products");
        }
    }
}

using Bogus;
using FluentAssertions;
using MongoDB.Driver;

namespace ProductService
{
    [Collection("Global Collection")]
    public class BaseRepositoryTests : IDisposable
    {
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<Product> _collection;
        private readonly TestBaseRepository _repository;
        private readonly Faker<Product> _productFaker;
        private bool _isDisposed = false;

        public BaseRepositoryTests()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(ToString());

            var client = new MongoClient("mongodb://localhost:27017");
            _repository = new TestBaseRepository(new ProductDbContext(client, "TestDatabase"));

            _productFaker = new Faker<Product>()
                .RuleFor(x => x.Name, "Test")
                .RuleFor(x => x.Id, Guid.NewGuid());

            _database = client.GetDatabase("TestDatabase");
            _collection = _database.GetCollection<Product>("TestEntities");
        }

        [Fact]
        public async Task CreateAsync_ShouldInsertEntity()
        {
            // Arrange
            var entity = _productFaker.Generate();

            // Act
            await _repository.CreateAsync(entity, CancellationToken.None);

            // Assert
            var insertedEntity = await _collection.Find(e => e.Id == entity.Id).FirstOrDefaultAsync();
            insertedEntity.Should().NotBeNull();
            insertedEntity.Name.Should().Be(entity.Name);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveEntity()
        {
            // Arrange
            var entity = _productFaker.Generate();
            await _repository.CreateAsync(entity, CancellationToken.None);

            // Act
            await _repository.DeleteAsync(entity, CancellationToken.None);

            // Assert
            var deletedEntity = await _collection.Find(e => e.Id == entity.Id).FirstOrDefaultAsync();
            deletedEntity.Should().BeNull();
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnEntity_WhenExists()
        {
            // Arrange
            var id = Guid.NewGuid();
            var entity = new Product { Id = id, Name = "Test" };
            await _repository.CreateAsync(entity, CancellationToken.None);

            // Act
            var result = await _repository.GetByIdAsync(id, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(entity);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenNotExists()
        {
            // Arrange
            var id = Guid.NewGuid();

            // Act
            var result = await _repository.GetByIdAsync(id, CancellationToken.None);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task UpdateAsync_ShouldReplaceEntity()
        {
            // Arrange
            var entity = _productFaker.Generate();
            await _repository.CreateAsync(entity, CancellationToken.None);
            entity.Name = "Updated Test";

            // Act
            await _repository.UpdateAsync(entity, CancellationToken.None);

            // Assert
            var updatedEntity = await _collection.Find(e => e.Id == entity.Id).FirstOrDefaultAsync();
            updatedEntity.Should().NotBeNull();
            updatedEntity.Name.Should().Be(entity.Name);
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                _isDisposed = true;
                _database.DropCollection("TestEntities");
            }
        }
    }
}

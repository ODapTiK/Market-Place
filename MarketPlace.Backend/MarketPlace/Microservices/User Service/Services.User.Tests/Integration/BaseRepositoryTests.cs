﻿using Bogus;
using FluentAssertions;

namespace UserService
{
    [Collection("Database Collection")]
    public class BaseRepositoryTests : IClassFixture<TestUserDatabaseFixture>
    {
        private readonly TestBaseRepository _testRepository;
        private readonly IUserDbContext _dbContext;
        private readonly Faker _faker;

        public BaseRepositoryTests(TestUserDatabaseFixture fixture)
        {
            _dbContext = fixture._context;
            _testRepository = new TestBaseRepository(_dbContext);
            _faker = new Faker();
        }

        [Fact]
        public async Task CreateAsync_ShouldAddEntity_WhenEntityIsCreated()
        {
            // Arrange
            var entity = new Admin { Id = Guid.NewGuid() };

            // Act
            var createdEntityId = await _testRepository.CreateAsync(entity, CancellationToken.None);

            // Assert
            createdEntityId.Should().Be(entity.Id);
            var savedEntity = await _dbContext.Set<Admin>().FindAsync(entity.Id);
            savedEntity.Should().NotBeNull();
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveEntity_WhenEntityIsDeleted()
        {
            // Arrange
            var entity = new Admin
            {
                Id = _faker.Random.Guid()
            };
            await _dbContext.Set<Admin>().AddAsync(entity);
            await _dbContext.SaveChangesAsync(CancellationToken.None);


            // Act
            await _testRepository.DeleteAsync(entity, CancellationToken.None);

            // Assert
            var deletedEntity = await _dbContext.Set<Admin>().FindAsync(entity.Id);
            deletedEntity.Should().BeNull();
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnEntity_WhenEntityExists()
        {
            // Arrange
            var entity = new Admin { Id = Guid.NewGuid() };
            await _dbContext.Set<Admin>().AddAsync(entity);
            await _dbContext.SaveChangesAsync(CancellationToken.None);

            // Act
            var result = await _testRepository.GetByIdAsync(entity.Id, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(entity);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenEntityDoesNotExist()
        {
            // Arrange
            var entity = new Admin { Id = Guid.NewGuid() };

            // Act
            var result = await _testRepository.GetByIdAsync(entity.Id, CancellationToken.None);

            // Assert
            result.Should().BeNull();
        }
    }
}

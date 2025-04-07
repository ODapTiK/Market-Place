using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace UserService
{
    [Collection("Database Collection")]
    public class ManufacturerRepositoryTests : IClassFixture<TestUserDatabaseFixture>
    {
        private readonly ManufacturerRepository _manufacturerRepository;
        private readonly IUserDbContext _context;

        public ManufacturerRepositoryTests(TestUserDatabaseFixture fixture)
        {
            _context = fixture._context;
            _manufacturerRepository = new ManufacturerRepository(_context);
        }

        [Fact]
        public async Task AddProductAsync_ShouldAddProductIdToManufacturer()
        {
            // Arrange
            var manufacturerId = Guid.NewGuid();
            var manufacturer = new Manufacturer
            {
                Id = manufacturerId,
                OrganizationProductsId = new List<Guid>()
            };
            await _context.Manufacturers.AddAsync(manufacturer);
            await _context.SaveChangesAsync(CancellationToken.None);

            var productId = Guid.NewGuid();

            // Act
            await _manufacturerRepository.AddProductAsync(manufacturer, productId, CancellationToken.None);

            // Assert
            var updatedManufacturer = await _context.Manufacturers.FindAsync(manufacturerId);
            updatedManufacturer.OrganizationProductsId.Should().Contain(productId);
        }

        [Fact]
        public async Task RemoveProductAsync_ShouldRemoveProductIdFromManufacturer()
        {
            // Arrange
            var manufacturerId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var manufacturer = new Manufacturer
            {
                Id = manufacturerId,
                OrganizationProductsId = new List<Guid> { productId }
            };
            await _context.Manufacturers.AddAsync(manufacturer);
            await _context.SaveChangesAsync(CancellationToken.None);

            // Act
            await _manufacturerRepository.RemoveProductAsync(manufacturer, productId, CancellationToken.None);

            // Assert
            var updatedManufacturer = await _context.Manufacturers.FindAsync(manufacturerId);
            updatedManufacturer.OrganizationProductsId.Should().NotContain(productId);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllManufacturers_WhenManufacturersExist()
        {
            // Arrange
            _context.Manufacturers.RemoveRange(await _context.Manufacturers.ToListAsync());
            var manufacturers = new List<Manufacturer>
            {
                new Manufacturer { Id = Guid.NewGuid(), Organization = "Manufacturer 1" },
                new Manufacturer { Id = Guid.NewGuid(), Organization = "Manufacturer 2" }
            };

            await _context.Manufacturers.AddRangeAsync(manufacturers);
            await _context.SaveChangesAsync(default);

            // Act
            var result = await _manufacturerRepository.GetAllAsync(CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.Should().Contain(m => m.Organization == "Manufacturer 1");
            result.Should().Contain(m => m.Organization == "Manufacturer 2");
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnEmptyList_WhenNoManufacturersExist()
        {
            //Assert
            _context.Manufacturers.RemoveRange(await _context.Manufacturers.ToListAsync());
            await _context.SaveChangesAsync(default);

            // Act
            var result = await _manufacturerRepository.GetAllAsync(CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }
    }
}

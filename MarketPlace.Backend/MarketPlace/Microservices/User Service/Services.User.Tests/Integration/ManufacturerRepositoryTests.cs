using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace UserService
{
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
    }
}

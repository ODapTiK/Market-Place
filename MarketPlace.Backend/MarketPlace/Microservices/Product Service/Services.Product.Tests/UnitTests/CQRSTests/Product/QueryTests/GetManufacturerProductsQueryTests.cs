using FluentAssertions;
using FluentValidation.TestHelper;
using Moq;
using System.Linq.Expressions;

namespace ProductService
{
    public class GetManufacturerProductsQueryTests
    {
        private readonly Mock<IProductRepository> _productRepositoryMock;
        private readonly GetManufacturerProductsQueryHandler _handler;

        public GetManufacturerProductsQueryTests()
        {
            _productRepositoryMock = new Mock<IProductRepository>();
            _handler = new GetManufacturerProductsQueryHandler(_productRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnProducts_WhenManufacturerHasProducts()
        {
            // Arrange
            var manufacturerId = Guid.NewGuid();
            var products = new List<Product>
            {
                new Product { Id = Guid.NewGuid(), Name = "Product 1", ManufacturerId = manufacturerId },
                new Product { Id = Guid.NewGuid(), Name = "Product 2", ManufacturerId = manufacturerId }
            };

            var query = new GetManufacturerProductsQuery { ManufacturerId = manufacturerId };

            _productRepositoryMock.Setup(repo => repo.GetManyProductsAsync(
                    It.IsAny<Expression<Func<Product, bool>>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(products);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(products); 
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyList_WhenManufacturerHasNoProducts()
        {
            // Arrange
            var manufacturerId = Guid.NewGuid();
            var query = new GetManufacturerProductsQuery { ManufacturerId = manufacturerId };

            _productRepositoryMock.Setup(repo => repo.GetManyProductsAsync(
                    It.IsAny<Expression<Func<Product, bool>>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Product>()); 

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeEmpty(); 
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenCommandIsInvalid()
        {
            // Arrange
            var command = new GetManufacturerProductsQuery
            {
                ManufacturerId = Guid.Empty
            };

            var validator = new GetManufacturerProductsQueryValidator();

            // Act
            var result = validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.ManufacturerId);
        }
    }
}

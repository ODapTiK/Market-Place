using FluentAssertions;
using Moq;

namespace ProductService
{
    public class GetAllProductsQueryTests
    {
        private readonly Mock<IProductRepository> _productRepositoryMock;
        private readonly GetAllProductsQueryHandler _handler;

        public GetAllProductsQueryTests()
        {
            _productRepositoryMock = new Mock<IProductRepository>();
            _handler = new GetAllProductsQueryHandler(_productRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnAllProducts_WhenProductsExist()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product { Id = Guid.NewGuid(), Name = "Product 1", Price = 10.0 },
                new Product { Id = Guid.NewGuid(), Name = "Product 2", Price = 20.0 }
            };

            var query = new GetAllProductsQuery();

            _productRepositoryMock.Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(products);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(products); 
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyList_WhenNoProductsExist()
        {
            // Arrange
            var query = new GetAllProductsQuery();

            _productRepositoryMock.Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Product>()); 

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeEmpty(); 
        }
    }
}

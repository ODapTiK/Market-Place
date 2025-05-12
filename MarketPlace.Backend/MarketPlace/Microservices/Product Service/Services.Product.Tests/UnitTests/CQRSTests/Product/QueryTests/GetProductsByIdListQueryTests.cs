using Bogus;
using FluentAssertions;
using Moq;
using System.Linq.Expressions;

namespace ProductService
{
    public class GetProductsByIdListQueryTests
    {
        private readonly Mock<IProductRepository> _productRepositoryMock;
        private readonly GetProductsByIdListQueryHandler _handler;
        private readonly Faker<Product> _productFaker;
        private readonly Faker<Review> _reviewFaker;

        public GetProductsByIdListQueryTests()
        {
            _productRepositoryMock = new Mock<IProductRepository>();
            _handler = new GetProductsByIdListQueryHandler(_productRepositoryMock.Object);

            _reviewFaker = new Faker<Review>()
                .RuleFor(r => r.Id, f => f.Random.Guid())
                .RuleFor(r => r.UserId, f => f.Random.Guid())
                .RuleFor(r => r.Description, f => f.Lorem.Sentences())
                .RuleFor(r => r.Raiting, f => f.Random.Int(1, 5));

            _productFaker = new Faker<Product>()
                .RuleFor(p => p.Id, f => f.Random.Guid())
                .RuleFor(p => p.ManufacturerId, f => f.Random.Guid())
                .RuleFor(p => p.Name, f => f.Commerce.ProductName())
                .RuleFor(p => p.Description, f => f.Commerce.ProductDescription())
                .RuleFor(p => p.Category, f => f.Commerce.Categories(1)[0])
                .RuleFor(p => p.Type, f => f.Commerce.ProductMaterial())
                .RuleFor(p => p.Reviews, f => _reviewFaker.Generate(f.Random.Int(0, 5)))
                .RuleFor(p => p.Image, f => f.Image.PicsumUrl())
                .RuleFor(p => p.Price, f => f.Random.Double(10, 1000))
                .RuleFor(p => p.Raiting, f => f.Random.Double(1, 5))
                .RuleFor(p => p.CreationDateTime, f => f.Date.Past());
        }

        [Fact]
        public async Task Handle_ShouldReturnProducts_WhenIdsExist()
        {
            // Arrange
            var existingProducts = _productFaker.Generate(3);
            var productIds = existingProducts.Select(p => p.Id).ToList();

            _productRepositoryMock.Setup(x => x.GetManyProductsAsync(
                    It.IsAny<Expression<Func<Product, bool>>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingProducts);

            var query = new GetProductsByIdListQuery { ProductIds = productIds };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(existingProducts.Count);
            result.Should().BeEquivalentTo(existingProducts);

            _productRepositoryMock.Verify(x => x.GetManyProductsAsync(
                It.IsAny<Expression<Func<Product, bool>>>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyList_WhenNoProductsFound()
        {
            // Arrange
            var productIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };

            _productRepositoryMock.Setup(x => x.GetManyProductsAsync(
                    It.IsAny<Expression<Func<Product, bool>>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Product>());

            var query = new GetProductsByIdListQuery { ProductIds = productIds };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task Handle_ShouldReturnOnlyRequestedProducts_WhenRepositoryReturnsMore()
        {
            // Arrange
            var allProducts = _productFaker.Generate(5);
            var requestedProductIds = allProducts.Take(2).Select(p => p.Id).ToList();
            var expectedProducts = allProducts.Where(p => requestedProductIds.Contains(p.Id)).ToList();

            _productRepositoryMock.Setup(x => x.GetManyProductsAsync(
                    It.IsAny<Expression<Func<Product, bool>>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedProducts);

            var query = new GetProductsByIdListQuery { ProductIds = requestedProductIds };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(expectedProducts.Count);
            result.Select(p => p.Id).Should().BeEquivalentTo(requestedProductIds);
        }

        [Fact]
        public async Task Handle_ShouldPassCancellationToken()
        {
            // Arrange
            var productIds = new List<Guid> { Guid.NewGuid() };
            var cancellationToken = new CancellationToken(true);

            _productRepositoryMock.Setup(x => x.GetManyProductsAsync(
                    It.IsAny<Expression<Func<Product, bool>>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Product>());

            var query = new GetProductsByIdListQuery { ProductIds = productIds };

            // Act
            await _handler.Handle(query, cancellationToken);

            // Assert
            _productRepositoryMock.Verify(x => x.GetManyProductsAsync(
                It.IsAny<Expression<Func<Product, bool>>>(),
                cancellationToken), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyList_WhenProductIdsIsEmpty()
        {
            // Arrange
            var query = new GetProductsByIdListQuery { ProductIds = new List<Guid>() };

            _productRepositoryMock.Setup(x => x.GetManyProductsAsync(
                It.IsAny<Expression<Func<Product, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Product>());

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();

            _productRepositoryMock.Verify(x => x.GetManyProductsAsync(
                It.IsAny<Expression<Func<Product, bool>>>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnProductsWithCompleteDataStructure()
        {
            // Arrange
            var testProduct = _productFaker.Generate();
            var productIds = new List<Guid> { testProduct.Id };

            _productRepositoryMock.Setup(x => x.GetManyProductsAsync(
                    It.IsAny<Expression<Func<Product, bool>>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Product> { testProduct });

            var query = new GetProductsByIdListQuery { ProductIds = productIds };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            var returnedProduct = result.First();
            returnedProduct.Should().NotBeNull();
            returnedProduct.Id.Should().NotBeEmpty();
            returnedProduct.ManufacturerId.Should().NotBeEmpty();
            returnedProduct.Name.Should().NotBeNull();
            returnedProduct.Description.Should().NotBeNull();
            returnedProduct.Category.Should().NotBeNull();
            returnedProduct.Type.Should().NotBeNull();
            returnedProduct.Image.Should().NotBeNull();
            returnedProduct.Price.Should().BePositive();
            returnedProduct.Raiting.Should().BeInRange(1, 5);
            returnedProduct.CreationDateTime.Should().BeBefore(DateTime.UtcNow);

            returnedProduct.Reviews.Should().NotBeNull();
            foreach (var review in returnedProduct.Reviews)
            {
                review.Id.Should().NotBeEmpty();
                review.UserId.Should().NotBeEmpty();
                review.Description.Should().NotBeNullOrEmpty();
                review.Raiting.Should().BeInRange(1, 5);
            }

            returnedProduct.ViewAt.Should().NotBeNull();
            foreach (var viewDate in returnedProduct.ViewAt)
            {
                viewDate.Should().BeBefore(DateTime.UtcNow);
            }
        }

        [Fact]
        public async Task Handle_ShouldReturnProductWithEmptyCollections_WhenNoData()
        {
            // Arrange
            var testProduct = _productFaker.Generate();
            testProduct.Reviews = new List<Review>();
            testProduct.ViewAt = new List<DateTime>();
            var productIds = new List<Guid> { testProduct.Id };

            _productRepositoryMock.Setup(x => x.GetManyProductsAsync(
                    It.IsAny<Expression<Func<Product, bool>>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Product> { testProduct });

            var query = new GetProductsByIdListQuery { ProductIds = productIds };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            var returnedProduct = result.First();
            returnedProduct.Reviews.Should().NotBeNull().And.BeEmpty();
            returnedProduct.ViewAt.Should().NotBeNull().And.BeEmpty();
        }
    }
}

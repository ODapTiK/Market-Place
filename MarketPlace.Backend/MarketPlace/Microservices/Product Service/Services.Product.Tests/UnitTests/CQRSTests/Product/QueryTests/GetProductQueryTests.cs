using FluentAssertions;
using FluentValidation.TestHelper;
using Moq;

namespace ProductService
{
    public class GetProductQueryTests
    {
        private readonly Mock<IProductRepository> _productRepositoryMock;
        private readonly GetProductQueryHandler _handler;

        public GetProductQueryTests()
        {
            _productRepositoryMock = new Mock<IProductRepository>();
            _handler = new GetProductQueryHandler(_productRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnProduct_WhenProductExists()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var product = new Product
            {
                Id = productId,
                ViewAt = new List<DateTime>()
            };

            var query = new GetProductQuery { Id = productId };

            _productRepositoryMock.Setup(repo => repo.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(product); 
            product.ViewAt.Should().ContainSingle().Which.Should().BeCloseTo(DateTime.Now.ToUniversalTime(), TimeSpan.FromSeconds(1)); 
        }

        [Fact]
        public async Task Handle_ShouldThrowEntityNotFoundException_WhenProductDoesNotExist()
        {
            // Arrange
            var query = new GetProductQuery { Id = Guid.NewGuid() };

            _productRepositoryMock.Setup(repo => repo.GetByIdAsync(query.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Product?)null); 

            // Act
            Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<EntityNotFoundException>();
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenCommandIsInvalid()
        {
            // Arrange
            var command = new GetProductQuery
            {
                Id = Guid.Empty
            };

            var validator = new GetProductQueryValidator();

            // Act
            var result = validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Id);
        }
    }
}

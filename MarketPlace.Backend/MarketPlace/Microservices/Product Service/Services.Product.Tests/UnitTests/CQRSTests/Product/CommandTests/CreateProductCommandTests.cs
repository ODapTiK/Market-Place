using Bogus;
using FluentAssertions;
using Moq;
using FluentValidation.TestHelper;

namespace ProductService
{
    public class CreateProductCommandTests
    {
        private readonly Mock<IProductRepository> _productRepositoryMock;
        private readonly CreateProductCommandHandler _handler;
        private readonly CreateProductCommandValidator _validator;

        public CreateProductCommandTests()
        {
            _productRepositoryMock = new Mock<IProductRepository>();
            _handler = new CreateProductCommandHandler(_productRepositoryMock.Object);
            _validator = new CreateProductCommandValidator();
        }

        [Fact]
        public async Task Handle_ShouldReturnNewProductId_WhenProductIsCreated()
        {
            // Arrange
            var faker = new Faker();
            var command = new CreateProductCommand
            {
                ManufacturerId = Guid.NewGuid(),
                Name = faker.Commerce.ProductName(),
                Description = faker.Commerce.ProductDescription(),
                Category = faker.Commerce.Department(),
                Type = faker.Commerce.ProductMaterial(),
                Image = Convert.ToBase64String(faker.Random.Bytes(64)),
                Price = Convert.ToDecimal(faker.Commerce.Price())
            };

            // Act
            var expectedProductId = Guid.NewGuid();
            _productRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedProductId);

            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().Be(expectedProductId);
            _productRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenCommandIsInvalid()
        {
            // Arrange
            var command = new CreateProductCommand
            {
                ManufacturerId = Guid.Empty,
                Price = -10m    
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.ManufacturerId);
            result.ShouldHaveValidationErrorFor(x => x.Price);
        }
    }
}

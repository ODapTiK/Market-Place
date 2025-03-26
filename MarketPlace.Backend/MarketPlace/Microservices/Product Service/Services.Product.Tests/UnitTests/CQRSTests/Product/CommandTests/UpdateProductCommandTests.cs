using FluentValidation.TestHelper;
using FluentAssertions;
using Moq;
using Bogus;

namespace ProductService
{
    public class UpdateProductCommandTests
    {
        private readonly Mock<IProductRepository> _productRepositoryMock;
        private readonly UpdateProductCommandHandler _handler;

        public UpdateProductCommandTests()
        {
            _productRepositoryMock = new Mock<IProductRepository>();
            _handler = new UpdateProductCommandHandler(_productRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldUpdateProduct_WhenProductExistsAndUserHasRights()
        {
            // Arrange
            var faker = new Faker();

            var productId = Guid.NewGuid();
            var manufacturerId = Guid.NewGuid();
            var command = new UpdateProductCommand
            {
                Id = productId,
                ManufacturerId = manufacturerId,
                Name = faker.Commerce.ProductName(),
                Description = faker.Commerce.ProductDescription(),
                Category = faker.Commerce.Department(),
                Type = faker.Commerce.ProductMaterial(),
                Image = Convert.ToBase64String(faker.Random.Bytes(64)),
                Price = Convert.ToDecimal(faker.Commerce.Price())
            };

            var product = new Product
            {
                Id = productId,
                ManufacturerId = manufacturerId,
                Name = faker.Commerce.ProductName(),
                Description = faker.Commerce.ProductDescription(),
                Category = faker.Commerce.Department(),
                Type = faker.Commerce.ProductMaterial(),
                Image = Convert.ToBase64String(faker.Random.Bytes(64)),
                Price = Convert.ToDecimal(faker.Commerce.Price())
            };

            _productRepositoryMock.Setup(repo => repo.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            product.Name.Should().Be(command.Name);
            product.Description.Should().Be(command.Description);
            product.Category.Should().Be(command.Category);
            product.Type.Should().Be(command.Type);
            product.Image.Should().Be(command.Image);
            product.Price.Should().Be(command.Price);
            _productRepositoryMock.Verify(repo => repo.UpdateAsync(product, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowEntityNotFoundException_WhenProductDoesNotExist()
        {
            // Arrange
            var command = new UpdateProductCommand
            {
                Id = Guid.NewGuid(),
                ManufacturerId = Guid.NewGuid(),
                Name = "Updated Product"
            };

            _productRepositoryMock.Setup(repo => repo.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Product?)null);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<EntityNotFoundException>();
        }

        [Fact]
        public async Task Handle_ShouldThrowLackOfRightException_WhenUserDoesNotHaveRights()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var command = new UpdateProductCommand
            {
                Id = productId,
                ManufacturerId = Guid.NewGuid()
            };

            var product = new Product
            {
                Id = productId,
                ManufacturerId = Guid.NewGuid()
            };

            _productRepositoryMock.Setup(repo => repo.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<LackOfRightException>();
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenCommandIsInvalid()
        {
            // Arrange
            var command = new UpdateProductCommand
            {
                Id = Guid.Empty,
                ManufacturerId = Guid.Empty,
                Price = -10
            };

            var validator = new UpdateProductCommandValidator();

            // Act
            var result = validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Id);
            result.ShouldHaveValidationErrorFor(x => x.ManufacturerId);
            result.ShouldHaveValidationErrorFor(x => x.Price);
        }
    }
}

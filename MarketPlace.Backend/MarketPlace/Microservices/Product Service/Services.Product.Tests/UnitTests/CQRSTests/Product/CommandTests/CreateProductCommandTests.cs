using Bogus;
using FluentAssertions;
using Moq;
using FluentValidation.TestHelper;
using Proto.ProductUser;

namespace ProductService
{
    public class CreateProductCommandTests
    {
        private readonly Mock<IProductRepository> _productRepositoryMock;
        private readonly CreateProductCommandHandler _handler;
        private readonly CreateProductCommandValidator _validator;
        private readonly Mock<ProductUserService.ProductUserServiceClient> _userServiceClientMock;

        public CreateProductCommandTests()
        {
            _productRepositoryMock = new Mock<IProductRepository>();
            _userServiceClientMock = new Mock<ProductUserService.ProductUserServiceClient>();
            _handler = new CreateProductCommandHandler(_productRepositoryMock.Object, 
                                                       _userServiceClientMock.Object);
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

            var mockCall = CallHelpers.CreateAsyncUnaryCall(new ProductResponse
            {
                Message = "Test",
                Success = true
            });
            _userServiceClientMock
                .Setup(m => m.AddManufacturerProductAsync(
                    It.IsAny<ProductRequest>(), null, null, CancellationToken.None))
                .Returns(mockCall);

            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().Be(expectedProductId);
            _productRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]

        public void Handle_ShouldThrowGRPCRequestFailException_WhenGRPCRequestReturnsFailure()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var command = new CreateProductCommand
            {
                ManufacturerId = userId,
                Price = 100m,
                Description = "Product description"
            };

            _productRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Guid.NewGuid());

            var mockCall = CallHelpers.CreateAsyncUnaryCall(new ProductResponse
            {
                Message = "Failure test",
                Success = false
            });
            _userServiceClientMock
                .Setup(m => m.AddManufacturerProductAsync(
                    It.IsAny<ProductRequest>(), null, null, CancellationToken.None))
                .Returns(mockCall);

            // Act
            var act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            act.Should().ThrowAsync<GRPCRequestFailException>().WithMessage("Failure test");
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

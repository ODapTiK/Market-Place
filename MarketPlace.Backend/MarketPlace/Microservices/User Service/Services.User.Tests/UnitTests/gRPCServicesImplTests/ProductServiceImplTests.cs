using Moq;
using FluentAssertions;
using Proto.ProductUser;
using Bogus;

namespace UserService
{
    public class ProductServiceImplTests
    {
        private readonly Mock<IAddManufacturerProductUseCase> _addManufacturerProductUseCaseMock;
        private readonly Mock<IRemoveManufacturerProductUseCase> _removeManufacturerProductUseCaseMock;
        private readonly ProductServiceImpl _productService;
        private readonly Faker<ProductRequest> _requestFaker;

        public ProductServiceImplTests()
        {
            _addManufacturerProductUseCaseMock = new Mock<IAddManufacturerProductUseCase>();
            _removeManufacturerProductUseCaseMock = new Mock<IRemoveManufacturerProductUseCase>();

            _requestFaker = new Faker<ProductRequest>()
                .RuleFor(x => x.ManufacturerId, Guid.NewGuid().ToString())
                .RuleFor(x => x.ProductId, Guid.NewGuid().ToString());

            _productService = new ProductServiceImpl(
                _addManufacturerProductUseCaseMock.Object,
                _removeManufacturerProductUseCaseMock.Object);
        }

        [Fact]
        public async Task AddManufacturerProduct_ShouldReturnSuccess_WhenProductIsAdded()
        {
            // Arrange
            var request = _requestFaker.Generate();
            var cancellationToken = new CancellationToken();

            _addManufacturerProductUseCaseMock.Setup(x => x.Execute(
                It.IsAny<Guid>(), It.IsAny<Guid>(), cancellationToken))
                .Returns(Task.CompletedTask);

            // Act
            var response = await _productService.AddManufacturerProduct(request, TestServerCallContext.Create());

            // Assert
            response.Should().NotBeNull();
            response.Success.Should().BeTrue();
            response.Message.Should().Contain($"Manufacturer(id: {request.ManufacturerId}) product(id: {request.ProductId}) added successfully");
        }

        [Fact]
        public async Task AddManufacturerProduct_ShouldReturnFailure_WhenExceptionIsThrown()
        {
            // Arrange
            var request = _requestFaker.Generate();
            var cancellationToken = new CancellationToken();

            _addManufacturerProductUseCaseMock.Setup(x => x.Execute(
                It.IsAny<Guid>(), It.IsAny<Guid>(), cancellationToken))
                .ThrowsAsync(new Exception("Failed to add product"));

            // Act
            var response = await _productService.AddManufacturerProduct(request, TestServerCallContext.Create());

            // Assert
            response.Should().NotBeNull();
            response.Success.Should().BeFalse();
            response.Message.Should().Be("Failed to add product");
        }

        [Fact]
        public async Task RemoveManufacturerProduct_ShouldReturnSuccess_WhenProductIsRemoved()
        {
            // Arrange
            var request = _requestFaker.Generate();
            var cancellationToken = new CancellationToken();

            _removeManufacturerProductUseCaseMock.Setup(x => x.Execute(
                It.IsAny<Guid>(), It.IsAny<Guid>(), cancellationToken))
                .Returns(Task.CompletedTask);

            // Act
            var response = await _productService.RemoveManufacturerProduct(request, TestServerCallContext.Create());

            // Assert
            response.Should().NotBeNull();
            response.Success.Should().BeTrue();
            response.Message.Should().Contain($"Manufacturer(id: {request.ManufacturerId}) product(id: {request.ProductId}) removed successfully");
        }

        [Fact]
        public async Task RemoveManufacturerProduct_ShouldReturnFailure_WhenExceptionIsThrown()
        {
            // Arrange
            var request = _requestFaker.Generate();
            var cancellationToken = new CancellationToken();

            _removeManufacturerProductUseCaseMock.Setup(x => x.Execute(
                It.IsAny<Guid>(), It.IsAny<Guid>(), cancellationToken))
                .ThrowsAsync(new Exception("Failed to remove product"));

            // Act
            var response = await _productService.RemoveManufacturerProduct(request, TestServerCallContext.Create());

            // Assert
            response.Should().NotBeNull();
            response.Success.Should().BeFalse();
            response.Message.Should().Be("Failed to remove product");
        }
    }
}

using Bogus;
using Grpc.Core;
using Moq;
using Proto.OrderProduct;
using FluentAssertions;

namespace ProductService
{
    public class OrderServiceImplTests
    {
        private readonly Mock<IProductRepository> _productRepositoryMock;
        private readonly OrderServiceImpl _service;
        private readonly Faker _faker;

        public OrderServiceImplTests()
        {
            _productRepositoryMock = new Mock<IProductRepository>();
            _service = new OrderServiceImpl(_productRepositoryMock.Object);
            _faker = new Faker();
        }

        [Fact]
        public async Task CalculateTotalPrice_ShouldReturnCorrectTotal_ForValidProducts()
        {
            // Arrange
            var product1 = new Product { Id = Guid.NewGuid(), Price = 10.5 };
            var product2 = new Product { Id = Guid.NewGuid(), Price = 20.0 };
            var product3 = new Product { Id = Guid.NewGuid(), Price = 15.75 };

            var request = new OrderRequest
            {
                OrderPoints =
                {
                    new OrderPoint { ProductId = product1.Id.ToString(), NumberOfUnits = 2 },
                    new OrderPoint { ProductId = product2.Id.ToString(), NumberOfUnits = 1 },
                    new OrderPoint { ProductId = product3.Id.ToString(), NumberOfUnits = 3 }
                }
            };

            var expectedTotal = (10.5 * 2) + (20.0 * 1) + (15.75 * 3);

            _productRepositoryMock.Setup(x => x.GetByIdAsync(product1.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(product1);
            _productRepositoryMock.Setup(x => x.GetByIdAsync(product2.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(product2);
            _productRepositoryMock.Setup(x => x.GetByIdAsync(product3.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(product3);

            // Act
            var response = await _service.CalculateTotalPrice(request, TestServerCallContext.Create());

            // Assert
            response.Should().NotBeNull();
            response.Success.Should().BeTrue();
            response.TotalPrice.Should().Be(expectedTotal);
            response.Message.Should().Be("Total price calculated successfully.");
        }

        [Fact]
        public async Task CalculateTotalPrice_ShouldReturnFailure_WhenProductNotFound()
        {
            // Arrange
            var missingProductId = Guid.NewGuid();
            var request = new OrderRequest
            {
                OrderPoints =
                {
                    new OrderPoint { ProductId = missingProductId.ToString(), NumberOfUnits = 1 }
                }
            };

            _productRepositoryMock.Setup(x => x.GetByIdAsync(missingProductId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Product?)null);

            // Act
            var response = await _service.CalculateTotalPrice(request, TestServerCallContext.Create());

            // Assert
            response.Should().NotBeNull();
            response.Success.Should().BeFalse();
            response.TotalPrice.Should().Be(0);
        }

        [Fact]
        public async Task CalculateTotalPrice_ShouldHandleEmptyRequest()
        {
            // Arrange
            var request = new OrderRequest(); 

            // Act
            var response = await _service.CalculateTotalPrice(request, TestServerCallContext.Create());

            // Assert
            response.Should().NotBeNull();
            response.Success.Should().BeTrue();
            response.TotalPrice.Should().Be(0);
            response.Message.Should().Be("Total price calculated successfully.");
        }

        [Fact]
        public async Task CalculateTotalPrice_ShouldHandleInvalidProductIdFormat()
        {
            // Arrange
            var request = new OrderRequest
            {
                OrderPoints = { new OrderPoint { ProductId = "invalid-guid", NumberOfUnits = 1 } }
            };

            // Act
            var response = await _service.CalculateTotalPrice(request, TestServerCallContext.Create());

            // Assert
            response.Should().NotBeNull();
            response.Success.Should().BeFalse();
        }
    }
}

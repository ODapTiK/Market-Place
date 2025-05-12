using FluentAssertions;
using FluentValidation.TestHelper;
using Moq;
using Proto.OrderProduct;
using Proto.ProductUser;

namespace ProductService
{
    public class DeleteProductCommandTests
    {
        private readonly Mock<IProductRepository> _productRepositoryMock;
        private readonly Mock<ProductUserService.ProductUserServiceClient> _userServiceClientMock;
        private readonly Mock<OrderProductService.OrderProductServiceClient> _orderProductServiceMock;
        private readonly DeleteProductCommandHandler _handler;

        public DeleteProductCommandTests()
        {
            _productRepositoryMock = new Mock<IProductRepository>();
            _userServiceClientMock = new Mock<ProductUserService.ProductUserServiceClient>();
            _orderProductServiceMock = new Mock<OrderProductService.OrderProductServiceClient>();
            _handler = new DeleteProductCommandHandler(_productRepositoryMock.Object, 
                                                       _userServiceClientMock.Object,
                                                       _orderProductServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldDeleteProduct_WhenProductExistsAndUserHasRights()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var manufacturerId = Guid.NewGuid();
            var command = new DeleteProductCommand
            {
                Id = productId,
                ManufacturerId = manufacturerId
            };

            var product = new Product
            {
                Id = productId,
                ManufacturerId = manufacturerId
            };

            _productRepositoryMock.Setup(repo => repo.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            var mockCall = CallHelpers.CreateAsyncUnaryCall(new ProductResponse
            {
                Message = "Test",
                Success = true
            });
            _userServiceClientMock
                .Setup(m => m.RemoveManufacturerProductAsync(
                    It.IsAny<Proto.ProductUser.ProductRequest>(), null, null, CancellationToken.None))
                .Returns(mockCall);

            var orderProductServiceMockCall = CallHelpers.CreateAsyncUnaryCall(new Response
            {
                Message = "Test",
                Success = true
            });
            _orderProductServiceMock
                .Setup(m => m.DeleteProductFromAllCartsAsync(
                    It.IsAny<DeleteProductRequest>(), null, null, CancellationToken.None))
                .Returns(orderProductServiceMockCall);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _productRepositoryMock.Verify(repo => repo.DeleteAsync(product, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowEntityNotFoundException_WhenProductDoesNotExist()
        {
            // Arrange
            var command = new DeleteProductCommand
            {
                Id = Guid.NewGuid(),
                ManufacturerId = Guid.NewGuid()
            };

            _productRepositoryMock.Setup(repo => repo.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Product?)null); 

            // Act
            var act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<EntityNotFoundException>();
        }

        [Fact]
        public async Task Handle_ShouldThrowLackOfRightException_WhenUserDoesNotHaveRights()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var command = new DeleteProductCommand
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
            var act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<LackOfRightException>();
        }

        [Fact]

        public void Handle_ShouldThrowGRPCRequestFailException_WhenGRPCRequestReturnsFailure()
        {
            var productId = Guid.NewGuid();
            var command = new DeleteProductCommand
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


            var mockCall = CallHelpers.CreateAsyncUnaryCall(new ProductResponse
            {
                Message = "Failure test",
                Success = false
            });
            _userServiceClientMock
                .Setup(m => m.RemoveManufacturerProductAsync(
                    It.IsAny<Proto.ProductUser.ProductRequest>(), null, null, CancellationToken.None))
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
            var command = new DeleteProductCommand
            {
                Id = Guid.Empty,
                ManufacturerId = Guid.Empty
            };

            var validator = new DeleteProductCommandValidator(); 

            // Act
            var result = validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Id);
            result.ShouldHaveValidationErrorFor(x => x.ManufacturerId);
        }
    }
}

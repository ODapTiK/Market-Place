using FluentAssertions;
using FluentValidation.TestHelper;
using Moq;

namespace ProductService
{
    public class DeleteProductCommandTests
    {
        private readonly Mock<IProductRepository> _productRepositoryMock;
        private readonly DeleteProductCommandHandler _handler;

        public DeleteProductCommandTests()
        {
            _productRepositoryMock = new Mock<IProductRepository>();
            _handler = new DeleteProductCommandHandler(_productRepositoryMock.Object);
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
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

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
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<LackOfRightException>();
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

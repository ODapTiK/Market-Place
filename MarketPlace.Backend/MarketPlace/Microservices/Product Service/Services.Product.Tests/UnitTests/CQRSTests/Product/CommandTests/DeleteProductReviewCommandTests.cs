using FluentAssertions;
using FluentValidation.TestHelper;
using Moq;

namespace ProductService
{
    public class DeleteProductReviewCommandTests
    {
        private readonly Mock<IProductRepository> _productRepositoryMock;
        private readonly DeleteProductReviewCommandHandler _handler;

        public DeleteProductReviewCommandTests()
        {
            _productRepositoryMock = new Mock<IProductRepository>();
            _handler = new DeleteProductReviewCommandHandler(_productRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldRemoveReview_WhenProductAndReviewExistAndUserHasRights()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var reviewId = Guid.NewGuid();

            var command = new DeleteProductReviewCommand
            {
                ProductId = productId,
                Id = reviewId,
                UserId = userId
            };

            var product = new Product
            {
                Id = productId,
                Reviews = new List<Review>
                {
                    new Review { Id = reviewId, UserId = userId, Raiting = 5, Description = "Great product!" }
                }
            };

            _productRepositoryMock.Setup(repo => repo.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            product.Reviews.Should().BeEmpty(); 
            _productRepositoryMock.Verify(repo => repo.UpdateAsync(product, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowEntityNotFoundException_WhenProductDoesNotExist()
        {
            // Arrange
            var command = new DeleteProductReviewCommand
            {
                ProductId = Guid.NewGuid(),
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid()
            };

            _productRepositoryMock.Setup(repo => repo.GetByIdAsync(command.ProductId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Product?)null); 

            // Act
            var act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<EntityNotFoundException>();
        }

        [Fact]
        public async Task Handle_ShouldThrowEntityNotFoundException_WhenReviewDoesNotExist()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var command = new DeleteProductReviewCommand
            {
                ProductId = productId,
                Id = Guid.NewGuid(), 
                UserId = userId
            };

            var product = new Product
            {
                Id = productId,
                Reviews = new List<Review>() 
            };

            _productRepositoryMock.Setup(repo => repo.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

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
            var reviewId = Guid.NewGuid();

            var command = new DeleteProductReviewCommand
            {
                ProductId = productId,
                Id = reviewId,
                UserId = Guid.NewGuid() 
            };

            var product = new Product
            {
                Id = productId,
                Reviews = new List<Review>
                {
                    new Review { Id = reviewId, UserId = Guid.NewGuid(), Raiting = 5, Description = "Great product!" } 
                }
            };

            _productRepositoryMock.Setup(repo => repo.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            // Act
            var act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<LackOfRightException>();
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenCommandIsInvalid()
        {
            // Arrange
            var command = new DeleteProductReviewCommand
            {
                ProductId = Guid.Empty,
                UserId = Guid.Empty,
                Id = Guid.Empty
            };

            var validator = new DeleteProductReviewCommandValidator();

            // Act
            var result = validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.ProductId);
            result.ShouldHaveValidationErrorFor(x => x.UserId);
            result.ShouldHaveValidationErrorFor(x => x.Id);
        }
    }
}

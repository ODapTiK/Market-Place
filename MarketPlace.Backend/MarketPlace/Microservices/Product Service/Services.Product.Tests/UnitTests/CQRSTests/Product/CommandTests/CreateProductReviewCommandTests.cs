using FluentAssertions;
using FluentValidation.TestHelper;
using Moq;

namespace ProductService
{
    public class CreateProductReviewCommandTests
    {
        private readonly Mock<IProductRepository> _productRepositoryMock;
        private readonly CreateProductReviewCommandHandler _handler;

        public CreateProductReviewCommandTests()
        {
            _productRepositoryMock = new Mock<IProductRepository>();
            _handler = new CreateProductReviewCommandHandler(_productRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldAddReview_WhenProductExists()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var command = new CreateProductReviewCommand
            {
                ProductId = productId,
                UserId = Guid.NewGuid(),
                Raiting = 5,
                Description = "Great product!"
            };

            var product = new Product
            {
                Id = productId,
                Reviews = new List<Review>()
            };

            _productRepositoryMock.Setup(repo => repo.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().Be(productId);
            product.Reviews.Should().HaveCount(1);
            product.Reviews.First().UserId.Should().Be(command.UserId);
            product.Reviews.First().Raiting.Should().Be(command.Raiting);
            product.Reviews.First().Description.Should().Be(command.Description);
            product.Raiting.Should().Be(5); 
            _productRepositoryMock.Verify(repo => repo.UpdateAsync(product, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowEntityNotFoundException_WhenProductDoesNotExist()
        {
            // Arrange
            var command = new CreateProductReviewCommand
            {
                ProductId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                Raiting = 5,
                Description = "Great product!"
            };

            _productRepositoryMock.Setup(repo => repo.GetByIdAsync(command.ProductId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Product?)null); 

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<EntityNotFoundException>();
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenCommandIsInvalid()
        {
            // Arrange
            var command = new CreateProductReviewCommand
            {
                ProductId = Guid.Empty,
                UserId = Guid.Empty,
                Raiting = 0, 
                Description = null 
            };

            var validator = new CreateProductReviewCommandValidator(); 

            // Act
            var result = validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.ProductId);
            result.ShouldHaveValidationErrorFor(x => x.UserId);
            result.ShouldHaveValidationErrorFor(x => x.Raiting);
            result.ShouldHaveValidationErrorFor(x => x.Description);
        }
    }
}

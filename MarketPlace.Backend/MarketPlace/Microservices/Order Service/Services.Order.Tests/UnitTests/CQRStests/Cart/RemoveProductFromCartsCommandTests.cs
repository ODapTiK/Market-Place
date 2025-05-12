using Bogus;
using FluentAssertions;
using Moq;

namespace OrderService
{
    public class RemoveProductFromCartsCommandTests
    {
        private readonly Mock<ICartRepository> _cartRepositoryMock;
        private readonly RemoveProductFromCartsCommandHandler _handler;
        private readonly Faker _faker;

        public RemoveProductFromCartsCommandTests()
        {
            _cartRepositoryMock = new Mock<ICartRepository>();
            _handler = new RemoveProductFromCartsCommandHandler(_cartRepositoryMock.Object);
            _faker = new Faker();
        }

        [Fact]
        public async Task Handle_ShouldCallRepositoryWithCorrectParameters()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var command = new RemoveProductFromCartsCommand()
            {
                ProductId = productId
            };
            var cancellationToken = new CancellationToken();

            // Act
            await _handler.Handle(command, cancellationToken);

            // Assert
            _cartRepositoryMock.Verify(
                x => x.RemoveProductFromCartsAsync(productId, cancellationToken),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldCompleteSuccessfully_WhenRepositorySucceeds()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var command = new RemoveProductFromCartsCommand()
            {
                ProductId = productId
            };

            _cartRepositoryMock.Setup(x => x.RemoveProductFromCartsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act & Assert
            await _handler.Invoking(x => x.Handle(command, CancellationToken.None))
                .Should().NotThrowAsync();
        }

        [Fact]
        public async Task Handle_ShouldPropagateException_WhenRepositoryFails()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var command = new RemoveProductFromCartsCommand()
            {
                ProductId = productId
            };
            var expectedException = new Exception("Database error");

            _cartRepositoryMock.Setup(x => x.RemoveProductFromCartsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(expectedException);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => _handler.Handle(command, CancellationToken.None));

            exception.Should().BeSameAs(expectedException);
        }
    }
}

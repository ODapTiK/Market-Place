using Bogus;
using FluentValidation.TestHelper;
using Moq;

namespace OrderService
{
    public class DeleteUserOrdersCommandTests 
    {
        private readonly Mock<IOrderRepository> _orderRepositoryMock;
        private readonly DeleteUserOrdersCommandHandler _handler;
        private readonly Faker _faker;
        private readonly DeleteUserOrdersCommandValidator _validator;
        public DeleteUserOrdersCommandTests()
        {
            _orderRepositoryMock = new Mock<IOrderRepository>();
            _handler = new DeleteUserOrdersCommandHandler(_orderRepositoryMock.Object);
            _faker = new Faker();
            _validator = new DeleteUserOrdersCommandValidator();
        }

        [Fact]
        public async Task Handle_ShouldDeleteOrder_WhenOrderExists()
        {
            // Arrange
            var userId = _faker.Random.Guid();
            var command = new DeleteUserOrdersCommand { UserId = userId };

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _orderRepositoryMock.Verify(repo => repo.DeleteUserOrdersAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenCommandIsInvalid()
        {
            // Arrange
            var command = new DeleteUserOrdersCommand
            {
                UserId = Guid.Empty
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.UserId);
        }
    }
}

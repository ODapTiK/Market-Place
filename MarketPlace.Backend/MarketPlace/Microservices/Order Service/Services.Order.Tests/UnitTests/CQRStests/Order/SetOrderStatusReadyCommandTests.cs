using Bogus;
using FluentAssertions;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using Moq;
using Proto.OrderUser;

namespace OrderService
{
    public class SetOrderStatusReadyCommandTests
    {
        private readonly Mock<IOrderRepository> _orderRepositoryMock;
        private readonly Mock<IObsoleteOrderCollector> _obsoleteOrderCollectorMock;
        private readonly Mock<IBackgroundJobClient> _backgroundJobClientMock;
        private readonly Mock<OrderUserService.OrderUserServiceClient> _orderUserServiceMock;
        private readonly Mock<IObsoleteOrdersClearingSettings> _obsoleteOrdersClearingSettingsMock;
        private readonly SetOrderStatusReadyCommandHandler _handler;
        private readonly Faker<Order> _orderFaker;

        public SetOrderStatusReadyCommandTests()
        {
            _orderRepositoryMock = new Mock<IOrderRepository>();
            _obsoleteOrderCollectorMock = new Mock<IObsoleteOrderCollector>();
            _backgroundJobClientMock = new Mock<IBackgroundJobClient>();
            _obsoleteOrdersClearingSettingsMock = new Mock<IObsoleteOrdersClearingSettings>();
            _orderUserServiceMock = new Mock<OrderUserService.OrderUserServiceClient>();

            _handler = new SetOrderStatusReadyCommandHandler(
                _orderRepositoryMock.Object,
                _obsoleteOrderCollectorMock.Object,
                _backgroundJobClientMock.Object,
                _orderUserServiceMock.Object,
                _obsoleteOrdersClearingSettingsMock.Object);

            _orderFaker = new Faker<Order>()
                .RuleFor(o => o.Id, f => f.Random.Guid())
                .RuleFor(o => o.UserId, f => f.Random.Guid())
                .RuleFor(o => o.Status, f => f.PickRandom("Pending", "Processing"))
                .RuleFor(o => o.ControlAdminId, f => f.Random.Guid())
                .RuleFor(o => o.OrderDateTime, f => f.Date.Recent());
        }

        [Fact]
        public async Task Handle_ShouldUpdateStatusToReady_WhenAdminHasRights()
        {
            // Arrange
            var adminId = Guid.NewGuid();
            var order = _orderFaker.Generate();
            order.ControlAdminId = adminId;

            var command = new SetOrderStatusReadyCommand()
            {
                AdminId = adminId,
                OrderId = order.Id
            };

            _orderRepositoryMock.Setup(x => x.GetByIdAsync(order.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);
            _orderRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            _obsoleteOrderCollectorMock.Setup(repo => repo.RemoveObsoleteOrderAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            var mockCall = CallHelpers.CreateAsyncUnaryCall(new Response
            {
                Message = "Success test",
                Success = true
            });
            _orderUserServiceMock
                .Setup(m => m.CreateOrderReadyNotificationAsync(
                    It.IsAny<OrderReadyRequest>(), null, null, CancellationToken.None))
                .Returns(mockCall);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            order.Status.Should().Be(OrderStatus.Ready.GetDisplayName());
            _orderRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<CancellationToken>()), Times.Once);
            _backgroundJobClientMock.Verify(x => x.Create(
                 It.Is<Job>(job => job.Method.Name == nameof(IObsoleteOrderCollector.RemoveObsoleteOrderAsync)),
                 It.IsAny<IState>()));
        }

        [Fact]
        public async Task Handle_ShouldScheduleObsoleteOrderRemoval_WhenStatusUpdated()
        {
            // Arrange
            var adminId = Guid.NewGuid();
            var order = _orderFaker.Generate();
            order.ControlAdminId = adminId;

            var command = new SetOrderStatusReadyCommand() 
            { 
                AdminId = adminId, 
                OrderId = order.Id 
            };

            _orderRepositoryMock.Setup(x => x.GetByIdAsync(order.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);
            _obsoleteOrderCollectorMock.Setup(repo => repo.RemoveObsoleteOrderAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            var mockCall = CallHelpers.CreateAsyncUnaryCall(new Response
            {
                Message = "Success test",
                Success = true
            });
            _orderUserServiceMock
                .Setup(m => m.CreateOrderReadyNotificationAsync(
                    It.IsAny<OrderReadyRequest>(), null, null, CancellationToken.None))
                .Returns(mockCall);


            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _backgroundJobClientMock.Verify(x => x.Create(
                It.Is<Job>(job => job.Method.Name == nameof(IObsoleteOrderCollector.RemoveObsoleteOrderAsync)),
                It.IsAny<IState>()));
        }

        [Fact]
        public async Task Handle_ShouldThrowEntityNotFoundException_WhenOrderNotFound()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var adminId = Guid.NewGuid();
            var command = new SetOrderStatusReadyCommand() 
            { 
                AdminId = adminId, 
                OrderId = orderId 
            };

            _orderRepositoryMock.Setup(x => x.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Order?)null);

            // Act & Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() =>
                _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldThrowLackOfRightsException_WhenAdminHasNoRights()
        {
            // Arrange
            var order = _orderFaker.Generate();
            var wrongAdminId = Guid.NewGuid();
            var command = new SetOrderStatusReadyCommand() 
            { 
                AdminId = wrongAdminId, 
                OrderId = order.Id 
            };

            _orderRepositoryMock.Setup(x => x.GetByIdAsync(order.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            // Act & Assert
            await Assert.ThrowsAsync<LackOfRightsException>(() =>
                _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldPassCancellationTokenToRepository()
        {
            // Arrange
            var adminId = Guid.NewGuid();
            var order = _orderFaker.Generate();
            order.ControlAdminId = adminId;
            var cancellationToken = new CancellationToken(true);

            var command = new SetOrderStatusReadyCommand() 
            { 
                AdminId = adminId, 
                OrderId = order.Id 
            };

            _orderRepositoryMock.Setup(x => x.GetByIdAsync(order.Id, cancellationToken))
                .ReturnsAsync(order);
            var mockCall = CallHelpers.CreateAsyncUnaryCall(new Response
            {
                Message = "Success test",
                Success = true
            });
            _orderUserServiceMock
                .Setup(m => m.CreateOrderReadyNotificationAsync(
                    It.IsAny<OrderReadyRequest>(), null, null, CancellationToken.None))
                .Returns(mockCall);

            // Act
            await _handler.Handle(command, cancellationToken);

            // Assert
            _orderRepositoryMock.Verify(x => x.GetByIdAsync(order.Id, cancellationToken), Times.Once);
        }
    }
}

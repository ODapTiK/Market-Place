using Hangfire;
using MediatR;
using Proto.OrderUser;

namespace OrderService
{
    public class SetOrderStatusReadyCommandHandler : IRequestHandler<SetOrderStatusReadyCommand>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IObsoleteOrderCollector _obsoleteOrderCollector;
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly OrderUserService.OrderUserServiceClient _orderUserServiceClient;

        public SetOrderStatusReadyCommandHandler(IOrderRepository orderRepository,
                                                 IObsoleteOrderCollector obsoleteOrderCollector,
                                                 IBackgroundJobClient backgroundJobClient,
                                                 OrderUserService.OrderUserServiceClient orderUserServiceClient)
        {
            _orderRepository = orderRepository;
            _obsoleteOrderCollector = obsoleteOrderCollector;
            _backgroundJobClient = backgroundJobClient;
            _orderUserServiceClient = orderUserServiceClient;
        }

        public async Task Handle(SetOrderStatusReadyCommand request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken)
                ?? throw new EntityNotFoundException(nameof(Order), request.OrderId);

            if (order.ControlAdminId != request.AdminId)
                throw new LackOfRightsException("Admin", request.AdminId, "Set order status 'Ready'");

            order.Status = OrderStatus.Ready.GetDisplayName();
            await _orderRepository.UpdateAsync(cancellationToken);

            var notificationRpcRequest = new OrderReadyRequest()
            {
                UserId = order.UserId.ToString(),
                OrderId = order.Id.ToString()
            };

            var notificationRpcResponse = await _orderUserServiceClient.CreateOrderReadyNotificationAsync(notificationRpcRequest);

            if (!notificationRpcResponse.Success)
                throw new GRPCRequestFailException(notificationRpcResponse.Message);

            _backgroundJobClient.Schedule(() => _obsoleteOrderCollector.RemoveObsoleteOrderAsync(order, cancellationToken), TimeSpan.FromDays(2));
        }
    }
}

using Hangfire;
using MediatR;

namespace OrderService
{
    public class SetOrderStatusReadyCommandHandler : IRequestHandler<SetOrderStatusReadyCommand>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IObsoleteOrderCollector _obsoleteOrderCollector;
        private readonly IBackgroundJobClient _backgroundJobClient;

        public SetOrderStatusReadyCommandHandler(IOrderRepository orderRepository,
                                                 IObsoleteOrderCollector obsoleteOrderCollector,
                                                 IBackgroundJobClient backgroundJobClient)
        {
            _orderRepository = orderRepository;
            _obsoleteOrderCollector = obsoleteOrderCollector;
            _backgroundJobClient = backgroundJobClient;
        }

        public async Task Handle(SetOrderStatusReadyCommand request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken)
                ?? throw new EntityNotFoundException(nameof(Order), request.OrderId);

            if (order.ControlAdminId != request.AdminId)
                throw new LackOfRightsException("Admin", request.AdminId, "Set order status 'Ready'");

            order.Status = OrderStatus.Ready.GetDisplayName();
            await _orderRepository.UpdateAsync(cancellationToken);

            _backgroundJobClient.Schedule(() => _obsoleteOrderCollector.RemoveObsoleteOrderAsync(order, cancellationToken), TimeSpan.FromDays(2));
        }
    }
}

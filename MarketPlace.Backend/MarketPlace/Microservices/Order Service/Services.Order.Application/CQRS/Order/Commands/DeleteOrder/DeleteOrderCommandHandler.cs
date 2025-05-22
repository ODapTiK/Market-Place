using Hangfire;
using MediatR;
using Proto.OrderUser;

namespace OrderService
{
    public class DeleteOrderCommandHandler : IRequestHandler<DeleteOrderCommand>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly OrderUserService.OrderUserServiceClient _userServiceClient;
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly IObsoleteOrderCollector _obsoleteOrderCollector;
        private readonly IObsoleteOrdersClearingSettings _obsoleteOrdersClearingSettings;

        public DeleteOrderCommandHandler(IOrderRepository orderRepository,
                                         OrderUserService.OrderUserServiceClient userServiceClient,
                                         IObsoleteOrderCollector obsoleteOrderCollector,
                                         IBackgroundJobClient backgroundJobClient,
                                         IObsoleteOrdersClearingSettings obsoleteOrdersClearingSettings)
        {
            _orderRepository = orderRepository;
            _userServiceClient = userServiceClient;
            _obsoleteOrderCollector = obsoleteOrderCollector;
            _backgroundJobClient = backgroundJobClient;
            _obsoleteOrdersClearingSettings = obsoleteOrdersClearingSettings;
        }

        public async Task Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetByIdAsync(request.Id, cancellationToken);
            if (order == null) 
                throw new EntityNotFoundException(nameof(Order), request.Id);

            await _orderRepository.DeleteAsync(order, cancellationToken);

            var rpcRequest = new OrderRequest
            {
                OrderId = request.Id.ToString(),
                UserId = order.UserId.ToString()
            };

            var rpcResponse = await _userServiceClient.RemoveUserOrderAsync(rpcRequest);

            if (!rpcResponse.Success)
                throw new GRPCRequestFailException(rpcResponse.Message);

            _backgroundJobClient.Schedule(() => _obsoleteOrderCollector.RemoveObsoleteOrderAsync(order, cancellationToken), 
                                          TimeSpan.FromHours(_obsoleteOrdersClearingSettings.RemoveOrdersRepeatTimeoutHours));
        }
    }
}

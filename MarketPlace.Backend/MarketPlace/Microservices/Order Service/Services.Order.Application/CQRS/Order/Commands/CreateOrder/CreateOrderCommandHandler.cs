using Hangfire;
using MediatR;
using Proto.OrderUser;

namespace OrderService
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Guid>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IObsoleteOrderCollector _obsoleteOrderCollector;
        private readonly OrderUserService.OrderUserServiceClient _orderUserServiceClient;
        private readonly IBackgroundJobClient _backgroundJobClient;

        public CreateOrderCommandHandler(IOrderRepository orderRepository,
                                         OrderUserService.OrderUserServiceClient orderUserServiceClient,
                                         IObsoleteOrderCollector obsoleteOrderCollector,
                                         IBackgroundJobClient backgroundJobClient)
        {
            _orderRepository = orderRepository;
            _orderUserServiceClient = orderUserServiceClient;
            _obsoleteOrderCollector = obsoleteOrderCollector;
            _backgroundJobClient = backgroundJobClient;
        }

        public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var order = new Order()
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                OrderPoints = request.Points,
                TotalPrice = request.TotalPrice,
                OrderDateTime = DateTime.Now.ToUniversalTime()
            };

            var orderId = await _orderRepository.CreateAsync(order, cancellationToken);

            var orderRpcRequest = new OrderRequest
            {
                OrderId = orderId.ToString(),
                UserId = request.UserId.ToString()
            };

            var rpcResponse = await _orderUserServiceClient.AddUserOrderAsync(orderRpcRequest);

            if(!rpcResponse.Success)
                throw new GRPCRequestFailException(rpcResponse.Message);

            _backgroundJobClient.Schedule(() => _obsoleteOrderCollector.RemoveObsoleteOrderAsync(order, cancellationToken), TimeSpan.FromDays(2));

            return orderId;
        }
    }
}

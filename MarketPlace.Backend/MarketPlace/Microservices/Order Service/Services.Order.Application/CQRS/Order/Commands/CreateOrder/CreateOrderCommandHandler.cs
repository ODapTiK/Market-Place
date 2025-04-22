using Hangfire;
using MediatR;
using Proto.OrderUser;
using Proto.OrderProduct;

namespace OrderService
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Guid>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IObsoleteOrderCollector _obsoleteOrderCollector;
        private readonly OrderUserService.OrderUserServiceClient _orderUserServiceClient;
        private readonly OrderProductService.OrderProductServiceClient _orderProductServiceClient;
        private readonly IBackgroundJobClient _backgroundJobClient;

        public CreateOrderCommandHandler(IOrderRepository orderRepository,
                                         OrderUserService.OrderUserServiceClient orderUserServiceClient,
                                         OrderProductService.OrderProductServiceClient orderProductServiceClient,
                                         IObsoleteOrderCollector obsoleteOrderCollector,
                                         IBackgroundJobClient backgroundJobClient)
        {
            _orderRepository = orderRepository;
            _orderUserServiceClient = orderUserServiceClient;
            _orderProductServiceClient = orderProductServiceClient;
            _obsoleteOrderCollector = obsoleteOrderCollector;
            _backgroundJobClient = backgroundJobClient;
        }

        public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var order = new Order()
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                OrderDateTime = DateTime.Now.ToUniversalTime()
            };

            var orderPoints = request.Points.Select(x => new OrderPoint()
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                ProductId = x.ProductId,
                NumberOfUnits = x.NumberOfUnits
            }).ToList();

            order.OrderPoints = orderPoints;

            var priceRpcRequest = new Proto.OrderProduct.OrderRequest();
            priceRpcRequest.OrderPoints.AddRange(orderPoints.Select(x => new Proto.OrderProduct.OrderPoint()
            {
                ProductId = x.ProductId.ToString(),
                NumberOfUnits = x.NumberOfUnits
            }));

            var priceRpcResponse = await _orderProductServiceClient.CalculateTotalPriceAsync(priceRpcRequest);
            if (!priceRpcResponse.Success) 
                throw new GRPCRequestFailException(priceRpcResponse.Message);

            order.TotalPrice = priceRpcResponse.TotalPrice;

            var orderId = await _orderRepository.CreateAsync(order, cancellationToken);

            var orderRpcRequest = new Proto.OrderUser.OrderRequest
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

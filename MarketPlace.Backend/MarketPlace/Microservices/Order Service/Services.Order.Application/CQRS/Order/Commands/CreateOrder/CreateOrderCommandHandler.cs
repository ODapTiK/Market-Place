using AutoMapper;
using MediatR;
using Proto.OrderUser;

namespace OrderService
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Guid>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly OrderUserService.OrderUserServiceClient _orderUserServiceClient;

        public CreateOrderCommandHandler(IOrderRepository orderRepository,
                                         OrderUserService.OrderUserServiceClient orderUserServiceClient)
        {
            _orderRepository = orderRepository;
            _orderUserServiceClient = orderUserServiceClient;
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

            return orderId;
        }
    }
}

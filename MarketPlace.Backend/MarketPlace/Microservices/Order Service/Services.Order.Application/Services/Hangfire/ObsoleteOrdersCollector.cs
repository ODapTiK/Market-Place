using Proto.OrderUser;

namespace OrderService
{
    public class ObsoleteOrdersCollector : IObsoleteOrderCollector
    {
        private readonly IOrderRepository _orderRepository;
        private readonly OrderUserService.OrderUserServiceClient _orderUserServiceClient;

        public ObsoleteOrdersCollector(IOrderRepository orderRepository,
                                       OrderUserService.OrderUserServiceClient orderUserServiceClient)
        {
            _orderRepository = orderRepository;
            _orderUserServiceClient = orderUserServiceClient;
        }

        public async Task RemoveObsoleteOrderAsync(Order order, CancellationToken cancellationToken)
        {
            await _orderRepository.DeleteAsync(order, cancellationToken);

            var rpcRequest = new RemoveObsoleteOrderFromUserAndAdminRequest()
            {
                AdminId = order.ControlAdminId.ToString(),
                UserId = order.UserId.ToString(),
                OrderId = order.Id.ToString()
            };

            var rpcResponse = await _orderUserServiceClient.RemoveObsoleteOrderFromUserAndAdminAsync(rpcRequest);

            if(!rpcResponse.Success) 
                throw new GRPCRequestFailException(rpcResponse.Message);
        }
    }
}

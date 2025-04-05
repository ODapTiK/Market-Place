using MediatR;
using Proto.OrderUser;

namespace OrderService
{
    public class DeleteOrderCommandHandler : IRequestHandler<DeleteOrderCommand>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly OrderUserService.OrderUserServiceClient _userServiceClient;

        public DeleteOrderCommandHandler(IOrderRepository orderRepository,
                                         OrderUserService.OrderUserServiceClient userServiceClient)
        {
            _orderRepository = orderRepository;
            _userServiceClient = userServiceClient;
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
        }
    }
}

using MediatR;

namespace OrderService
{
    public class DeleteUserOrdersCommandHandler : IRequestHandler<DeleteUserOrdersCommand>
    {
        private readonly IOrderRepository _orderRepository;

        public DeleteUserOrdersCommandHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task Handle(DeleteUserOrdersCommand request, CancellationToken cancellationToken)
        {
            await _orderRepository.DeleteUserOrdersAsync(request.UserId, cancellationToken);    
        }
    }
}

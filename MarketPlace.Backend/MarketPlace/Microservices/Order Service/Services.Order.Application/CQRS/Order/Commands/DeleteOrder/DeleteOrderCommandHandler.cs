using MediatR;

namespace OrderService
{
    public class DeleteOrderCommandHandler : IRequestHandler<DeleteOrderCommand>
    {
        private readonly IOrderRepository _orderRepository;

        public DeleteOrderCommandHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetByIdAsync(request.Id, cancellationToken);
            if (order == null) 
                throw new EntityNotFoundException(nameof(Order), request.Id);

            await _orderRepository.DeleteAsync(order, cancellationToken);
        }
    }
}

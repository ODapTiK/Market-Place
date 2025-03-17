using MediatR;

namespace OrderService
{
    public class GetOrderQueryHandler : IRequestHandler<GetOrderQuery, Order>
    {
        private readonly IOrderRepository _orderRepository;

        public GetOrderQueryHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<Order> Handle(GetOrderQuery request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetByIdAsync(request.Id, cancellationToken);
            if (order == null) 
                throw new EntityNotFoundException(nameof(Order), request.Id);

            return order;
        }
    }
}

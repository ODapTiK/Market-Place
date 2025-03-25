using MediatR;

namespace OrderService
{
    public class GetUserOrdersQueryHandler : IRequestHandler<GetUserOrdersQuery, List<Order>>
    {
        private readonly IOrderRepository _orderRepository;

        public GetUserOrdersQueryHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public Task<List<Order>> Handle(GetUserOrdersQuery request, CancellationToken cancellationToken)
        {
            var orders = _orderRepository.GetUserOrdersAsync(request.UserId, cancellationToken);

            return orders;
        }
    }
}

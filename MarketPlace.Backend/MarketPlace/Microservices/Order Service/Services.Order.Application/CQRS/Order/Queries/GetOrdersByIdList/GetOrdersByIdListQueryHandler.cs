using MediatR;

namespace OrderService
{
    public class GetOrdersByIdListQueryHandler : IRequestHandler<GetOrdersByIdListQuery, List<Order>>
    {
        private readonly IOrderRepository _orderRepository;

        public GetOrdersByIdListQueryHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<List<Order>> Handle(GetOrdersByIdListQuery request, CancellationToken cancellationToken)
        {
            return await _orderRepository.GetManyOrdersAsync(x => request.OrderIds.Contains(x.Id), cancellationToken);
        }
    }
}

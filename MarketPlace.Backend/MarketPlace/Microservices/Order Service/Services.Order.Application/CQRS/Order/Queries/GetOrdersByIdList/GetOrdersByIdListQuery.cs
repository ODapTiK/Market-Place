using MediatR;

namespace OrderService
{
    public class GetOrdersByIdListQuery : IRequest<List<Order>>
    {
        public List<Guid> OrderIds { get; set; } = [];
    }
}

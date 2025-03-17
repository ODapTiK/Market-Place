using MediatR;

namespace OrderService
{
    public class GetUserOrdersQuery : IRequest<List<Order>>
    {
        public Guid UserId { get; set; }
    }
}

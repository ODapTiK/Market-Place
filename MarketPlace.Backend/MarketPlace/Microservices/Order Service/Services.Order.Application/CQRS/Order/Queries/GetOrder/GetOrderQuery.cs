using MediatR;

namespace OrderService
{
    public class GetOrderQuery : IRequest<Order>
    {
        public Guid Id { get; set; }
    }
}

using MediatR;

namespace OrderService
{
    public class CreateOrderCommand : IRequest<Guid>
    {
        public Guid UserId { get; set; }
        public List<OrderPoint> Points { get; set; } = [];
        public decimal TotalPrice { get; set; }
    }
}

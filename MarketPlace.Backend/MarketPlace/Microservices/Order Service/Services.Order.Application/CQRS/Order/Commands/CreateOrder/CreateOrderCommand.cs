using MediatR;

namespace OrderService
{
    public class CreateOrderCommand : IRequest<Guid>
    {
        public Guid UserId { get; set; }
        public List<OrderPointDTO> Points { get; set; } = [];
    }
}

using MediatR;

namespace OrderService
{
    public class SetOrderStatusReadyCommand : IRequest
    {
        public Guid AdminId { get; set; }
        public Guid OrderId { get; set; }
    }
}

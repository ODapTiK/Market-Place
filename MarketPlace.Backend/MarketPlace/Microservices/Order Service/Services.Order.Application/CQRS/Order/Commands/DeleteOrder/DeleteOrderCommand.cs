using MediatR;

namespace OrderService
{
    public class DeleteOrderCommand : IRequest
    {
        public Guid Id { get; set; }
    }
}

using MediatR;

namespace OrderService
{
    public class AddOrderPointCommand : IRequest
    {
        public Guid CartId { get; set; }
        public Guid ProductId { get; set; }
    }
}

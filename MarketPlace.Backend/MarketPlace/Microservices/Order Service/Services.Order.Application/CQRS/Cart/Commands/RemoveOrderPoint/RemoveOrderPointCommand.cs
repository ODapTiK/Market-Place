using MediatR;

namespace OrderService
{
    public class RemoveOrderPointCommand : IRequest
    {
        public Guid CartId { get; set; }
        public Guid ProductId { get; set; }
    }
}

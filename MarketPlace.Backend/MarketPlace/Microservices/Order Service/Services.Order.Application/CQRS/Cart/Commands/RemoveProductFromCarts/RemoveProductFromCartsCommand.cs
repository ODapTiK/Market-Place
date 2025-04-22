using MediatR;

namespace OrderService
{
    public class RemoveProductFromCartsCommand : IRequest
    {
        public Guid ProductId { get; set; }
    }
}

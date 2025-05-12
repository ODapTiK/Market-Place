using MediatR;

namespace ProductService
{
    public class RemoveProductFromUserCartCommand : IRequest
    {
        public Guid ProductId { get; set; }
        public Guid UserId { get; set; }
    }
}

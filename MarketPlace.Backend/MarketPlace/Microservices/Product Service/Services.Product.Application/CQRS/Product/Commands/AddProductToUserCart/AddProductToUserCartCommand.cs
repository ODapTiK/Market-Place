using MediatR;

namespace ProductService
{
    public class AddProductToUserCartCommand : IRequest
    {
        public Guid ProductId { get; set; }
        public Guid UserId { get; set; }
    }
}

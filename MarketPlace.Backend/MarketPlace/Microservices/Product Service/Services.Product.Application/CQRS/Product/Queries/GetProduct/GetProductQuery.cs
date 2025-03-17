using MediatR;

namespace ProductService
{
    public class GetProductQuery : IRequest<Product>
    {
        public Guid Id { get; set; }
    }
}

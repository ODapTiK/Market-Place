using MediatR;

namespace ProductService
{
    public class DeleteProductCommand : IRequest
    {
        public Guid Id { get; set; }
        public Guid ManufacturerId { get; set; }
    }
}

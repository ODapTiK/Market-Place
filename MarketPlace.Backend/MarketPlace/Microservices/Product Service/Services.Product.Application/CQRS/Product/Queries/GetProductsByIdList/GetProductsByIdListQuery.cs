using MediatR;

namespace ProductService
{
    public class GetProductsByIdListQuery : IRequest<List<Product>>
    {
        public List<Guid> ProductIds { get; set; } = [];
    }
}

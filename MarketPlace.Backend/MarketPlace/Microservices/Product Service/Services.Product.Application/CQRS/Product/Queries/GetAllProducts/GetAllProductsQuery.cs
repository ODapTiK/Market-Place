using MediatR;

namespace ProductService
{
    public class GetAllProductsQuery : IRequest<List<Product>>
    {
    }
}

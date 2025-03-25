using MediatR;

namespace ProductService
{
    public class GetManufacturerProductsQuery : IRequest<List<Product>>
    {
        public Guid ManufacturerId { get; set; }
    }
}

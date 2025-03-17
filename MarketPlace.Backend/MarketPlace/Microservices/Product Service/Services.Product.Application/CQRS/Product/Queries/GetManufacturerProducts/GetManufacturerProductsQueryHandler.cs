using MediatR;

namespace ProductService
{
    public class GetManufacturerProductsQueryHandler : IRequestHandler<GetManufacturerProductsQuery, List<Product>>
    {
        private readonly IProductRepository _productRepository;

        public GetManufacturerProductsQueryHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<List<Product>> Handle(GetManufacturerProductsQuery request, CancellationToken cancellationToken)
        {
            return await _productRepository.GetManyProductsAsync(x => x.ManufacturerId.Equals(request.ManufacturerId), cancellationToken);
        }
    }
}

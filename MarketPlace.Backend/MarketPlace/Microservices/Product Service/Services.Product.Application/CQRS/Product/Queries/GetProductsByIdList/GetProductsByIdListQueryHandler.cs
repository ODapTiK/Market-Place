using MediatR;

namespace ProductService
{
    public class GetProductsByIdListQueryHandler : IRequestHandler<GetProductsByIdListQuery, List<Product>>
    {
        private readonly IProductRepository _productRepository;

        public GetProductsByIdListQueryHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<List<Product>> Handle(GetProductsByIdListQuery request, CancellationToken cancellationToken)
        {
            return await _productRepository.GetManyProductsAsync(x => request.ProductIds.Contains(x.Id), cancellationToken);
        }
    }
}

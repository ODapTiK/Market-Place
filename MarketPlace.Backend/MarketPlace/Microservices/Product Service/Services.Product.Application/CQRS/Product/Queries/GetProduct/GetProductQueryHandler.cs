using MediatR;

namespace ProductService
{
    public class GetProductQueryHandler : IRequestHandler<GetProductQuery, Product>
    {
        private readonly IProductRepository _productRepository;

        public GetProductQueryHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<Product> Handle(GetProductQuery request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetByIdAsync(request.Id, cancellationToken);
            if(product == null) 
                throw new EntityNotFoundException(nameof(Product), request.Id); 

            product.ViewAt.Add(DateTime.Now.ToUniversalTime());

            return product;
        }
    }
}

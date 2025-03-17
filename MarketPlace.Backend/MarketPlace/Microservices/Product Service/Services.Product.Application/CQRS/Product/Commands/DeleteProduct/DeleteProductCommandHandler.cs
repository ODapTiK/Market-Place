using MediatR;

namespace ProductService
{
    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand>
    {
        private readonly IProductRepository _productRepository;

        public DeleteProductCommandHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetByIdAsync(request.Id, cancellationToken);
            if (product == null)
                throw new EntityNotFoundException(nameof(Product), request.Id);
            else if (product.ManufacturerId != request.ManufacturerId)
                throw new LackOfRightException(nameof(Product), request.ManufacturerId, "Deleteasync  Product");

            await _productRepository.DeleteAsync(product, cancellationToken);
        }
    }
}

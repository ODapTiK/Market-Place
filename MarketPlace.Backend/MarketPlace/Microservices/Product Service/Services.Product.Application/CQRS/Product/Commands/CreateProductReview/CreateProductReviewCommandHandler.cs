using MediatR;

namespace ProductService
{
    public class CreateProductReviewCommandHandler : IRequestHandler<CreateProductReviewCommand, Guid>
    {
        private readonly IProductRepository _productRepository;

        public CreateProductReviewCommandHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<Guid> Handle(CreateProductReviewCommand request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);
            if (product == null) 
                throw new EntityNotFoundException(nameof(Product), request.ProductId);

            var review = new Review()
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                Raiting = request.Raiting,
                Description = request.Description,
            };

            product.Reviews.Add(review);
            product.Raiting = product.Reviews.Average(x => x.Raiting);

            await _productRepository.UpdateAsync(product, cancellationToken);
            return product.Id;
        }
    }
}

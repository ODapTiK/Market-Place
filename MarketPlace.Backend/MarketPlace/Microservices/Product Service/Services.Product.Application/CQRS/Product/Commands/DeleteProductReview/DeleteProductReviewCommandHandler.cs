using MediatR;

namespace ProductService
{
    public class DeleteProductReviewCommandHandler : IRequestHandler<DeleteProductReviewCommand>
    {
        private readonly IProductRepository _productRepository;

        public DeleteProductReviewCommandHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task Handle(DeleteProductReviewCommand request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);
            if (product == null) 
                throw new EntityNotFoundException(nameof(Product), request.ProductId);
            
            var reviewToRemove = product.Reviews.FirstOrDefault(r => r.Id == request.Id);
            if (reviewToRemove == null)
                throw new EntityNotFoundException(nameof(Review), request.Id);

            if(reviewToRemove.UserId != request.UserId)
                throw new LackOfRightException("User", request.UserId, "Remove review");

            product.Reviews.Remove(reviewToRemove);
            await _productRepository.UpdateAsync(product, cancellationToken);
        }
    }
}

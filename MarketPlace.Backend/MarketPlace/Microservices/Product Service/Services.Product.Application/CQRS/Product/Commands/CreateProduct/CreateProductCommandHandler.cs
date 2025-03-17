using MediatR;

namespace ProductService
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Guid>
    {
        private readonly IProductRepository _productRepository;

        public CreateProductCommandHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var product = new Product()
            {
                Id = Guid.NewGuid(),
                ManufacturerId = request.ManufacturerId,
                Name = request.Name,
                Description = request.Description,
                Category = request.Category,
                Type = request.Type,
                Image = request.Image,
                Price = request.Price,
                Reviews = [],
                Raiting = 0
            };

            return await _productRepository.CreateAsync(product, cancellationToken);
        }
    }
}

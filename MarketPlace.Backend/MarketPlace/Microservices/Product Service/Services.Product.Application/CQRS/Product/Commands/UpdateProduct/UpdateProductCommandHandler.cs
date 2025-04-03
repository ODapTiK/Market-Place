using MediatR;

namespace ProductService
{
    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand>
    {
        private readonly IProductRepository _productRepository;
        private readonly IRabbitMqProducerService _rabbitMqProducerService;

        public UpdateProductCommandHandler(IProductRepository productRepository,
                                           IRabbitMqProducerService rabbitMqProducerService)
        {
            _productRepository = productRepository;
            _rabbitMqProducerService = rabbitMqProducerService;
        }

        public async Task Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetByIdAsync(request.Id, cancellationToken);
            if (product == null) 
                throw new EntityNotFoundException(nameof(Product), request.Id);
            else if (product.ManufacturerId != request.ManufacturerId)
                throw new LackOfRightException(nameof(Product), request.ManufacturerId, "Update Product");

            product.Name = request.Name;
            product.Description = request.Description;
            product.Category = request.Category;
            product.Type = request.Type;
            product.Image = request.Image;
            product.Price = request.Price;

            await _productRepository.UpdateAsync(product, cancellationToken);

            await _rabbitMqProducerService.SendMessage(product.Id.ToString(), "UpdatedProducts");
        }
    }
}

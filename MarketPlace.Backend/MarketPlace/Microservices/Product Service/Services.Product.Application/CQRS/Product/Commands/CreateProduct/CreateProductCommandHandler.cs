using MediatR;
using Proto.ProductUser;

namespace ProductService
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Guid>
    {
        private readonly IProductRepository _productRepository;
        private readonly ProductUserService.ProductUserServiceClient _userServiceClient;

        public CreateProductCommandHandler(IProductRepository productRepository,
                                           ProductUserService.ProductUserServiceClient client)
        {
            _productRepository = productRepository;
            _userServiceClient = client;
        }

        public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var product = new Product()
            {
                Id = Guid.NewGuid(),
                CreationDateTime = DateTime.Now.ToUniversalTime(),
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

            var id =  await _productRepository.CreateAsync(product, cancellationToken);

            var productRequest = new ProductRequest
            {
                ManufacturerId = request.ManufacturerId.ToString(),
                ProductId = id.ToString()
            };

            var rpcResponse = await _userServiceClient.AddManufacturerProductAsync(productRequest);

            if (!rpcResponse.Success)
                throw new GRPCRequestFailException(rpcResponse.Message);

            return id;
        }
    }
}

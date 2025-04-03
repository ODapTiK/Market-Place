using MediatR;
using Proto.ProductUser;

namespace ProductService
{
    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand>
    {
        private readonly IProductRepository _productRepository;
        private readonly ProductUserService.ProductUserServiceClient _userServiceClient;

        public DeleteProductCommandHandler(IProductRepository productRepository, 
                                           ProductUserService.ProductUserServiceClient userServiceClient)
        {
            _productRepository = productRepository;
            _userServiceClient = userServiceClient;
        }

        public async Task Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetByIdAsync(request.Id, cancellationToken);
            if (product == null)
                throw new EntityNotFoundException(nameof(Product), request.Id);
            else if (product.ManufacturerId != request.ManufacturerId)
                throw new LackOfRightException(nameof(Product), request.ManufacturerId, "Delete Product");

            await _productRepository.DeleteAsync(product, cancellationToken);

            var rpcRequest = new ProductRequest
            {
                ProductId = product.Id.ToString(),
                ManufacturerId = product.ManufacturerId.ToString()
            };

            var rpcResponse = await _userServiceClient.RemoveManufacturerProductAsync(rpcRequest);

            if(!rpcResponse.Success)
                throw new GRPCRequestFailException(rpcResponse.Message);
        }
    }
}

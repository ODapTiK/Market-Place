using MediatR;
using Proto.OrderProduct;
using Proto.ProductUser;

namespace ProductService
{
    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand>
    {
        private readonly IProductRepository _productRepository;
        private readonly ProductUserService.ProductUserServiceClient _userServiceClient;
        private readonly OrderProductService.OrderProductServiceClient _orderServiceClient;

        public DeleteProductCommandHandler(IProductRepository productRepository, 
                                           ProductUserService.ProductUserServiceClient userServiceClient,
                                           OrderProductService.OrderProductServiceClient orderServiceClient)
        {
            _productRepository = productRepository;
            _userServiceClient = userServiceClient;
            _orderServiceClient = orderServiceClient;
        }

        public async Task Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetByIdAsync(request.Id, cancellationToken);
            if (product == null)
                throw new EntityNotFoundException(nameof(Product), request.Id);
            else if (product.ManufacturerId != request.ManufacturerId)
                throw new LackOfRightException(nameof(Product), request.ManufacturerId, "Delete Product");

            await _productRepository.DeleteAsync(product, cancellationToken);

            var deleteProductFromManufacturerRpcRequest = new Proto.ProductUser.ProductRequest
            {
                ProductId = product.Id.ToString(),
                ManufacturerId = product.ManufacturerId.ToString()
            };

            var deleteProductFromCartsRpcRequest = new DeleteProductRequest()
            {
                ProductId = product.Id.ToString()
            };

            var manufacturerRpcResponse = _userServiceClient.RemoveManufacturerProductAsync(deleteProductFromManufacturerRpcRequest).ResponseAsync;
            var cartsRpcResponse = _orderServiceClient.DeleteProductFromAllCartsAsync(deleteProductFromCartsRpcRequest).ResponseAsync;

            await Task.WhenAll(manufacturerRpcResponse, cartsRpcResponse);

            if(!manufacturerRpcResponse.Result.Success)
                throw new GRPCRequestFailException(manufacturerRpcResponse.Result.Message);
            if(!cartsRpcResponse.Result.Success)
                throw new GRPCRequestFailException(cartsRpcResponse.Result.Message);
        }
    }
}

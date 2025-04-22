using MediatR;
using Proto.OrderProduct;

namespace ProductService
{
    public class AddProductToUserCartCommandHandler : IRequestHandler<AddProductToUserCartCommand>
    {
        private readonly OrderProductService.OrderProductServiceClient _orderServiceClient;
        private readonly IProductRepository _productRepository;

        public AddProductToUserCartCommandHandler(OrderProductService.OrderProductServiceClient orderServiceClient, 
                                                  IProductRepository productRepository)
        {
            _orderServiceClient = orderServiceClient;
            _productRepository = productRepository;
        }
        public async Task Handle(AddProductToUserCartCommand request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken)
                ?? throw new EntityNotFoundException(nameof(Product), request.ProductId);

            var rpcRequest = new ProductRequest()
            {
                ProductId = product.Id.ToString(),
                UserId = request.UserId.ToString()
            };

            var rpcResponse = await _orderServiceClient.AddProductToCartAsync(rpcRequest);

            if(!rpcResponse.Success) 
                throw new GRPCRequestFailException(rpcResponse.Message);
        }
    }
}

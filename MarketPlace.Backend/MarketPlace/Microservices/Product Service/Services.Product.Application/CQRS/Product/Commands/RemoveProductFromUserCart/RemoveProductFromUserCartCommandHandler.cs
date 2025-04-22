using MediatR;
using Proto.OrderProduct;

namespace ProductService
{
    public class RemoveProductFromUserCartCommandHandler : IRequestHandler<RemoveProductFromUserCartCommand>
    {
        private readonly OrderProductService.OrderProductServiceClient _orderClientService;
        private readonly IProductRepository _productRepository;

        public RemoveProductFromUserCartCommandHandler(OrderProductService.OrderProductServiceClient orderClientService, 
                                                IProductRepository productRepository)
        {
            _orderClientService = orderClientService;
            _productRepository = productRepository;
        }
        public async Task Handle(RemoveProductFromUserCartCommand request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken)
                ?? throw new EntityNotFoundException(nameof(Product), request.ProductId);

            var rpcRequest = new ProductRequest()
            {
                ProductId = product.Id.ToString(),
                UserId = request.UserId.ToString(),
            };

            var rpcResponse = await _orderClientService.RemoveProductFromCartAsync(rpcRequest);

            if (!rpcResponse.Success)
                throw new GRPCRequestFailException(rpcResponse.Message);
        }
    }
}

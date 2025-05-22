using Grpc.Core;
using Proto.OrderProduct;

namespace ProductService
{
    public class OrderServiceImpl : OrderProductService.OrderProductServiceBase
    {
        private readonly IProductRepository _productRepository;

        public OrderServiceImpl(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public override async Task<ProductInfoResponse> GetProductInfo(GetProductInfoRequest request, ServerCallContext context)
        {
            try
            {
                var product = await _productRepository.GetByIdAsync(Guid.Parse(request.ProductId), context.CancellationToken) ??
                    throw new EntityNotFoundException(nameof(Product), request.ProductId);

                return new ProductInfoResponse()
                {
                    Success = true,
                    Message = "Product info got successfully",
                    ProductName = product.Name ?? "",
                    ProductDescription = product.Description ?? "",
                    ProductCategory = product.Category ?? "",
                    ProductImage = product.Image ?? "",
                    ProductType = product.Type ?? ""
                };
            }
            catch(Exception ex)
            {
                return new ProductInfoResponse()
                {
                    Success = false,
                    Message = ex.Message,
                    ProductName = null,
                    ProductDescription = null,
                    ProductCategory = null,
                    ProductImage = null,
                    ProductType = null
                };
            }
        }
        public override async Task<OrderResponse> CalculateTotalPrice(OrderRequest request, ServerCallContext context)
        {
            double totalPrice = 0.0;
            try
            {
                foreach (var orderPoint in request.OrderPoints)
                {
                    var product = await _productRepository.GetByIdAsync(Guid.Parse(orderPoint.ProductId), context.CancellationToken)
                        ?? throw new EntityNotFoundException(nameof(Product), orderPoint.ProductId);
                    totalPrice += product.Price * orderPoint.NumberOfUnits;
                }

                var response = new OrderResponse
                {
                    Success = true,
                    Message = "Total price calculated successfully.",
                    TotalPrice = totalPrice
                };

                return response;
            }
            catch (Exception ex)
            {
                var response = new OrderResponse
                {
                    Success = false,
                    Message = ex.Message,
                    TotalPrice = 0
                };

                return response;
            }
        }
    }
}

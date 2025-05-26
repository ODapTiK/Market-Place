using Hangfire;
using MediatR;
using Proto.OrderUser;
using Proto.OrderProduct;

namespace OrderService
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Guid>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly OrderUserService.OrderUserServiceClient _orderUserServiceClient;
        private readonly OrderProductService.OrderProductServiceClient _orderProductServiceClient;

        public CreateOrderCommandHandler(IOrderRepository orderRepository,
                                         OrderUserService.OrderUserServiceClient orderUserServiceClient,
                                         OrderProductService.OrderProductServiceClient orderProductServiceClient)
        {
            _orderRepository = orderRepository;
            _orderUserServiceClient = orderUserServiceClient;
            _orderProductServiceClient = orderProductServiceClient;
        }

        public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var order = new Order()
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                ControlAdminId = request.ControlAdminId,
                OrderDateTime = DateTime.Now.ToUniversalTime(),
                Status = OrderStatus.InProgress.GetDisplayName()
            };

            var orderPoints = new List<OrderPoint>();
            foreach (var orderPoint in request.Points) {
                var productInfoRequest = new GetProductInfoRequest()
                {
                    ProductId = orderPoint.ProductId.ToString()
                };

                var productInfoResponse = await _orderProductServiceClient.GetProductInfoAsync(productInfoRequest);

                if(!productInfoResponse.Success) 
                    throw new GRPCRequestFailException(productInfoResponse.Message);

                orderPoints.Add(new OrderPoint()
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.Id,
                    ProductId = orderPoint.ProductId,
                    productCategory = productInfoResponse.ProductCategory == "" ? null : productInfoResponse.ProductCategory,
                    productDescription = productInfoResponse.ProductDescription == "" ? null : productInfoResponse.ProductDescription,
                    productImage = productInfoResponse.ProductImage == "" ? null : productInfoResponse.ProductImage,
                    productName = productInfoResponse.ProductName == "" ? null : productInfoResponse.ProductName,
                    productType = productInfoResponse.ProductType == "" ? null : productInfoResponse.ProductType,
                    NumberOfUnits = orderPoint.NumberOfUnits
                });
            }

            order.OrderPoints = orderPoints;

            var priceRpcRequest = new Proto.OrderProduct.OrderRequest();
            priceRpcRequest.OrderPoints.AddRange(orderPoints.Select(x => new Proto.OrderProduct.OrderPoint()
            {
                ProductId = x.ProductId.ToString(),
                NumberOfUnits = x.NumberOfUnits
            }));

            var priceRpcResponse = await _orderProductServiceClient.CalculateTotalPriceAsync(priceRpcRequest);
            if (!priceRpcResponse.Success) 
                throw new GRPCRequestFailException(priceRpcResponse.Message);

            order.TotalPrice = priceRpcResponse.TotalPrice;

            var orderId = await _orderRepository.CreateAsync(order, cancellationToken);

            var orderRpcRequest = new Proto.OrderUser.OrderRequest
            {
                OrderId = orderId.ToString(),
                UserId = request.UserId.ToString()
            };

            var rpcResponse = await _orderUserServiceClient.AddUserOrderAsync(orderRpcRequest);

            if(!rpcResponse.Success)
                throw new GRPCRequestFailException(rpcResponse.Message);

            var controlAdminRpcRequest = new AddOrderToControlAdminRequest()
            {
                AdminId = order.ControlAdminId.ToString(),
                OrderId = order.Id.ToString()
            };

            var controlAdminRpcResponse = await _orderUserServiceClient.AddOrderToControlAdminAsync(controlAdminRpcRequest);

            if (!controlAdminRpcResponse.Success)
                throw new GRPCRequestFailException(controlAdminRpcResponse.Message);

            return orderId;
        }
    }
}

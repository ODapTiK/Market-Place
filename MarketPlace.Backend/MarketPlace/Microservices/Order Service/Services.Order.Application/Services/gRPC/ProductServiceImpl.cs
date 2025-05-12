using Grpc.Core;
using MediatR;
using Proto.OrderProduct;

namespace OrderService
{
    public class ProductServiceImpl : OrderProductService.OrderProductServiceBase
    {
        private readonly IMediator _mediator;

        public ProductServiceImpl(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override async Task<Response> AddProductToCart(ProductRequest request, ServerCallContext context)
        {
            try
            {
                var getUserCartQuery = new GetUserCartQuery()
                {
                    UserId = Guid.Parse(request.UserId)
                };

                var userCart = await _mediator.Send(getUserCartQuery);

                var command = new AddOrderPointCommand()
                {
                    ProductId = Guid.Parse(request.ProductId),
                    CartId = userCart.Id
                };

                await _mediator.Send(command);

                return new Response
                {
                    Success = true,
                    Message = $"Product \"{request.ProductId}\" added to cart \"{userCart.Id}\" successfully"
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    Success = false,
                    Message = ex.Message
                };

                throw;
            }

        }

        public override async Task<Response> RemoveProductFromCart(ProductRequest request, ServerCallContext context)
        {
            try
            {
                var getUserCartQuery = new GetUserCartQuery()
                {
                    UserId = Guid.Parse(request.UserId)
                };

                var userCart = await _mediator.Send(getUserCartQuery);

                var command = new RemoveOrderPointCommand()
                {
                    ProductId = Guid.Parse(request.ProductId),
                    CartId = userCart.Id
                };

                await _mediator.Send(command);

                return new Response
                {
                    Success = true,
                    Message = $"Product \"{request.ProductId}\" removed from cart \"{userCart.Id}\" successfully"
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    Success = false,
                    Message = ex.Message
                };

                throw;
            }
        }

        public override async Task<Response> DeleteProductFromAllCarts(DeleteProductRequest request, ServerCallContext context)
        {
            try
            {
                var command = new RemoveProductFromCartsCommand()
                {
                    ProductId = Guid.Parse(request.ProductId),
                };

                await _mediator.Send(command);

                return new Response
                {
                    Success = true,
                    Message = $"Product \"{request.ProductId}\" removed from carts successfully"
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    Success = false,
                    Message = ex.Message
                };

                throw;
            }
        }
    }
}

using Grpc.Core;
using MediatR;
using Proto.OrderUser;

namespace OrderService
{
    public class UserServiceImpl : OrderUserService.OrderUserServiceBase
    {
        private readonly IMediator _mediator;

        public UserServiceImpl(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override async Task<Response> DeleteUserOrders(DeleteUserOrdersRequest request, ServerCallContext context)
        {
            var command = new DeleteUserOrdersCommand()
            {
                UserId = Guid.Parse(request.UserId)
            };

            try
            {
                await _mediator.Send(command);

                return new Response
                {
                    Success = true,
                    Message = $"User \"{request.UserId}\" orders removed successfully"
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
        public override async Task<Response> CreateCart(CartRequest request, ServerCallContext context)
        {
            var command = new CreateCartCommand()
            {
                UserId = Guid.Parse(request.UserId)
            };

            try
            {
                var id = await _mediator.Send(command);

                return new Response
                {
                    Success = true,
                    Message = $"Cart \"{id}\" for user \"{request.UserId}\" created successfully"
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

        public override async Task<Response> DeleteCart(CartRequest request, ServerCallContext context)
        {
            var command = new DeleteCartCommand()
            {
                UserId = Guid.Parse(request.UserId)
            };

            try
            {
                await _mediator.Send(command);

                return new Response
                {
                    Success = true,
                    Message = $"User\"{request.UserId}\" cart deleted successfully"
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

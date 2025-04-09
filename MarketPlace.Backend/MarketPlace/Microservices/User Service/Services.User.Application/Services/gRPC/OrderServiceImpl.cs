using Grpc.Core;
using Proto.OrderUser;

namespace UserService
{
    public class OrderServiceImpl : OrderUserService.OrderUserServiceBase
    {
        private readonly IAddUserOrderUseCase _addUserOrderUseCase;
        private readonly IRemoveUserOrderUseCase _removeUserOrderUseCase;

        public OrderServiceImpl(IAddUserOrderUseCase addUserOrderUseCase,
                                IRemoveUserOrderUseCase removeUserOrderUseCase)
        {
            _addUserOrderUseCase = addUserOrderUseCase;
            _removeUserOrderUseCase = removeUserOrderUseCase;
        }

        public override async Task<Response> AddUserOrder(OrderRequest request, ServerCallContext context)
        {
            try
            {
                await _addUserOrderUseCase.Execute(Guid.Parse(request.UserId),
                                                   Guid.Parse(request.OrderId),
                                                   context.CancellationToken);

                return new Response
                {
                    Success = true,
                    Message = $"Order \"{request.OrderId}\" added to user \"{request.UserId}\" successfully"
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

        public override async Task<Response> RemoveUserOrder(OrderRequest request, ServerCallContext context)
        {
            try
            {
                await _removeUserOrderUseCase.Execute(Guid.Parse(request.UserId),
                                                   Guid.Parse(request.OrderId),
                                                   context.CancellationToken);

                return new Response
                {
                    Success = true,
                    Message = $"Order \"{request.OrderId}\" removed from user \"{request.UserId}\" successfully"
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

using Grpc.Core;
using Proto.OrderUser;

namespace UserService
{
    public class OrderServiceImpl : OrderUserService.OrderUserServiceBase
    {
        private readonly IAddUserOrderUseCase _addUserOrderUseCase;
        private readonly IRemoveUserOrderUseCase _removeUserOrderUseCase;
        private readonly IRemoveControlAdminOrderUseCase _removeControlAdminOrderUseCase;
        private readonly IAddOrderToControlAdminUseCase _addOrderToControlAdminUseCase;

        public OrderServiceImpl(IAddUserOrderUseCase addUserOrderUseCase,
                                IRemoveUserOrderUseCase removeUserOrderUseCase,
                                IAddOrderToControlAdminUseCase addOrderToControlAdminUseCase,
                                IRemoveControlAdminOrderUseCase removeControlAdminOrderUseCase)
        {
            _addUserOrderUseCase = addUserOrderUseCase;
            _removeUserOrderUseCase = removeUserOrderUseCase;
            _addOrderToControlAdminUseCase = addOrderToControlAdminUseCase;
            _removeControlAdminOrderUseCase = removeControlAdminOrderUseCase;
        }

        public override async Task<Response> AddOrderToControlAdmin(AddOrderToControlAdminRequest request, ServerCallContext context)
        {
            try
            {
                await _addOrderToControlAdminUseCase.Execute(Guid.Parse(request.AdminId),
                                                   Guid.Parse(request.OrderId),
                                                   context.CancellationToken);

                return new Response
                {
                    Success = true,
                    Message = $"Order \"{request.OrderId}\" added to admin \"{request.AdminId}\" successfully"
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

        public override async Task<Response> RemoveObsoleteOrderFromUserAndAdmin(RemoveObsoleteOrderFromUserAndAdminRequest request, ServerCallContext context)
        {
            try
            {
                await _removeControlAdminOrderUseCase.Execute(Guid.Parse(request.AdminId),
                                                   Guid.Parse(request.OrderId),
                                                   context.CancellationToken);
                await _removeUserOrderUseCase.Execute(Guid.Parse(request.UserId),
                                                   Guid.Parse(request.OrderId),
                                                   context.CancellationToken);

                return new Response
                {
                    Success = true,
                    Message = $"Order \"{request.OrderId}\" removed from admin \"{request.AdminId}\" and user \"{request.UserId}\" successfully"
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

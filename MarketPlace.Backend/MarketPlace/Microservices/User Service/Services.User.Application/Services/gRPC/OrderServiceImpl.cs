using Grpc.Core;
using Microsoft.AspNetCore.SignalR;
using Proto.OrderUser;

namespace UserService
{
    public class OrderServiceImpl : OrderUserService.OrderUserServiceBase
    {
        private readonly IAddUserOrderUseCase _addUserOrderUseCase;
        private readonly IRemoveUserOrderUseCase _removeUserOrderUseCase;
        private readonly IRemoveControlAdminOrderUseCase _removeControlAdminOrderUseCase;
        private readonly IAddOrderToControlAdminUseCase _addOrderToControlAdminUseCase;
        private readonly IAddUserNotificationUseCase _addUserNotificationUseCase;
        private readonly IAddAdminNotificationUseCase _addAdminNotificationUseCase;
        private readonly IHubContext<NotificationHub> _notificationHub;

        public OrderServiceImpl(IAddUserOrderUseCase addUserOrderUseCase,
                                IRemoveUserOrderUseCase removeUserOrderUseCase,
                                IAddOrderToControlAdminUseCase addOrderToControlAdminUseCase,
                                IRemoveControlAdminOrderUseCase removeControlAdminOrderUseCase,
                                IAddUserNotificationUseCase addUserNotificationUseCase,
                                IAddAdminNotificationUseCase addAdminNotificationUseCase,
                                IHubContext<NotificationHub> notificationHub)
        {
            _addUserOrderUseCase = addUserOrderUseCase;
            _removeUserOrderUseCase = removeUserOrderUseCase;
            _addOrderToControlAdminUseCase = addOrderToControlAdminUseCase;
            _removeControlAdminOrderUseCase = removeControlAdminOrderUseCase;
            _addUserNotificationUseCase = addUserNotificationUseCase;
            _addAdminNotificationUseCase = addAdminNotificationUseCase;
            _notificationHub = notificationHub;
        }

        public override async Task<Response> CreateOrderReadyNotification(OrderReadyRequest request, ServerCallContext context)
        {
            try
            {
                var notification = new Notification
                {
                    Title = "Order ready",
                    Message = $"Your order({request.OrderId}) completed",
                    CreatedAt = DateTime.Now.ToUniversalTime(),
                    IsRead = false
                };

                await _addUserNotificationUseCase.Execute(Guid.Parse(request.UserId), notification, context.CancellationToken);

                await _notificationHub.Clients.Group(request.UserId)
                    .SendAsync("ReceiveNotification", notification);

                return new Response()
                {
                    Success = true,
                    Message = "Notofications send successfully"
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

        public override async Task<Response> CreateUpdateProductNotification(UpdateProductNotificationsRequest request, ServerCallContext context)
        {
            try
            {
                foreach (var userId in request.UserId)
                {
                    var notification = new Notification
                    {
                        Title = "Product Updated",
                        Message = $"A product in your order has been updated (ID: {request.ProductId})",
                        Type = "product-update",
                        CreatedAt = DateTime.Now.ToUniversalTime(),
                        IsRead = false
                    };

                    await _addUserNotificationUseCase.Execute(Guid.Parse(userId), notification, context.CancellationToken);

                    await _notificationHub.Clients.Group(userId.ToString())
                        .SendAsync("ReceiveNotification", notification);
                }

                return new Response()
                {
                    Success = true,
                    Message = "Notofications send successfully"
                };
            }
            catch(Exception ex)
            {
                return new Response
                {
                    Success = false,
                    Message = ex.Message
                };

                throw;
            }
        }

        public override async Task<Response> AddOrderToControlAdmin(AddOrderToControlAdminRequest request, ServerCallContext context)
        {
            try
            {
                await _addOrderToControlAdminUseCase.Execute(Guid.Parse(request.AdminId),
                                                   Guid.Parse(request.OrderId),
                                                   context.CancellationToken);

                var notification = new Notification
                {
                    Title = "New order awaiting confirmation",
                    Message = $"Order #{request.OrderId} is awaiting control confirmation",
                    CreatedAt = DateTime.Now.ToUniversalTime(),
                    IsRead = false
                };

                await _addAdminNotificationUseCase.Execute(Guid.Parse(request.AdminId), notification, context.CancellationToken);

                await _notificationHub.Clients.Group(request.AdminId)
                    .SendAsync("ReceiveNotification", notification);

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

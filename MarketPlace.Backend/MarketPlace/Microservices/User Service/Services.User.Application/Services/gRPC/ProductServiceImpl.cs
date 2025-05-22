using Grpc.Core;
using Microsoft.AspNetCore.SignalR;
using Proto.ProductUser;
using System.Text;

namespace UserService
{
    public class ProductServiceImpl : ProductUserService.ProductUserServiceBase
    {
        private readonly IAddManufacturerProductUseCase _addManufacturerProductUseCase;
        private readonly IRemoveManufacturerProductUseCase _removeManufacturerProductUseCase;
        private readonly IGetManufacturersIdUseCase _getManufacturersIdUseCase;
        private readonly IAddManufacturerNotificationUseCase _addManufacturerNotificationUseCase;
        private readonly IHubContext<NotificationHub> _notificationHub;

        public ProductServiceImpl(IAddManufacturerProductUseCase addManufacturerProductUseCase, 
                                  IRemoveManufacturerProductUseCase removeManufacturerProductUseCase,
                                  IGetManufacturersIdUseCase getManufacturersIdUseCase,
                                  IAddManufacturerNotificationUseCase addManufacturerNotificationUseCase,
                                  IHubContext<NotificationHub> hubContext)
        {
            _addManufacturerProductUseCase = addManufacturerProductUseCase;
            _removeManufacturerProductUseCase = removeManufacturerProductUseCase;
            _getManufacturersIdUseCase = getManufacturersIdUseCase;
            _addManufacturerNotificationUseCase = addManufacturerNotificationUseCase;
            _notificationHub = hubContext;
        }

        public override async Task<ProductResponse> CreateManufacturersDailyReport(ManufacturersDailyReportRequest request, ServerCallContext context)
        {
            try
            {
                foreach (var manufacturer in request.Reports)
                {
                        var message = new StringBuilder();
                        foreach (var view in manufacturer.ProductsViews)
                        {
                            message.AppendLine($"Product {view.Key}: {view.Value} views");
                        }
                        message.AppendLine($"Total views: {manufacturer.ProductsViews.Select(x => x.Value).Sum()}");
                        var notification = new Notification
                        {
                            Title = "Daily report",
                            Message = message.ToString(),
                            CreatedAt = DateTime.Now.ToUniversalTime(),
                            IsRead = false
                        };

                    await _addManufacturerNotificationUseCase.Execute(Guid.Parse(manufacturer.ManufacturerId), notification, context.CancellationToken);

                    await _notificationHub.Clients.Group(manufacturer.ManufacturerId)
                        .SendAsync("ReceiveNotification", notification);
                }

                return new ProductResponse()
                {
                    Success = true,
                    Message = "Notofications send successfully"
                };
            }
            catch (Exception ex)
            {
                return new ProductResponse
                {
                    Success = false,
                    Message = ex.Message
                };

                throw;
            }
        }

        public override async Task<ManufacturerResponse> GetManufacturers(ManufacturersRequest request, ServerCallContext context)
        {
            try
            {
                var manufacturersId = await _getManufacturersIdUseCase.Execute(context.CancellationToken);
                var response = new ManufacturerResponse()
                {
                    Success = true,
                    Message = "Manufacturer IDs successfully received"
                };
                response.ManufacturerId.AddRange(manufacturersId.Select(x => x.ToString()).ToList());

                return response;
            }
            catch (Exception ex)
            {
                var response = new ManufacturerResponse()
                {
                    Success = false,
                    Message = ex.Message,
                };
                response.ManufacturerId.AddRange([]);

                return response;
            }
        }

        public override async Task<ProductResponse> AddManufacturerProduct(ProductRequest request, ServerCallContext context)
        {
            try
            {
                await _addManufacturerProductUseCase.Execute(
                    Guid.Parse(request.ManufacturerId),
                    Guid.Parse(request.ProductId),
                    context.CancellationToken);
            }
            catch (Exception ex)
            {
                return new ProductResponse
                {
                    Success = false,
                    Message = ex.Message
                };

                throw;
            }

            return new ProductResponse
            {
                Success = true,
                Message = $"Manufacturer(id: {request.ManufacturerId}) product(id: {request.ProductId}) added successfully"
            };
        }

        public override async Task<ProductResponse> RemoveManufacturerProduct(ProductRequest request, ServerCallContext context)
        {
            try
            {
                await _removeManufacturerProductUseCase.Execute(
                    Guid.Parse(request.ManufacturerId),
                    Guid.Parse(request.ProductId),
                    context.CancellationToken);
            }
            catch (Exception ex)
            {
                return new ProductResponse
                {
                    Success = false,
                    Message = ex.Message
                };

                throw;
            }

            return new ProductResponse
            {
                Success = true,
                Message = $"Manufacturer(id: {request.ManufacturerId}) product(id: {request.ProductId}) removed successfully"
            };
        }
    }
}

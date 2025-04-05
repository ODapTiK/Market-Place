using Grpc.Core;
using Proto.ProductUser;

namespace UserService
{
    public class ProductServiceImpl : ProductUserService.ProductUserServiceBase
    {
        private readonly IAddManufacturerProductUseCase _addManufacturerProductUseCase;
        private readonly IRemoveManufacturerProductUseCase _removeManufacturerProductUseCase;

        public ProductServiceImpl(IAddManufacturerProductUseCase addManufacturerProductUseCase, 
                                  IRemoveManufacturerProductUseCase removeManufacturerProductUseCase)
        {
            _addManufacturerProductUseCase = addManufacturerProductUseCase;
            _removeManufacturerProductUseCase = removeManufacturerProductUseCase;
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

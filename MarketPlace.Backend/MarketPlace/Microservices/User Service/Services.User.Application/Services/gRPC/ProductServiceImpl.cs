using Grpc.Core;
using Proto.ProductUser;

namespace UserService
{
    public class ProductServiceImpl : ProductUserService.ProductUserServiceBase
    {
        private readonly IAddManufacturerProductUseCase _addManufacturerProductUseCase;
        private readonly IRemoveManufacturerProductUseCase _removeManufacturerProductUseCase;
        private readonly IGetManufacturersIdUseCase _getManufacturersIdUseCase;

        public ProductServiceImpl(IAddManufacturerProductUseCase addManufacturerProductUseCase, 
                                  IRemoveManufacturerProductUseCase removeManufacturerProductUseCase,
                                  IGetManufacturersIdUseCase getManufacturersIdUseCase)
        {
            _addManufacturerProductUseCase = addManufacturerProductUseCase;
            _removeManufacturerProductUseCase = removeManufacturerProductUseCase;
            _getManufacturersIdUseCase = getManufacturersIdUseCase;
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

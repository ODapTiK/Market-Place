using Proto.ProductUser;

namespace ProductService
{
    public class DailyProductsReportsGenerator
    {
        private readonly ProductUserService.ProductUserServiceClient _productUserServiceClient;
        private readonly IProductRepository _productRepository;

        public DailyProductsReportsGenerator(ProductUserService.ProductUserServiceClient productUserServiceClient,
                                             IProductRepository productRepository)
        {
            _productUserServiceClient = productUserServiceClient;
            _productRepository = productRepository;
        }
        public async Task GenerateDailyReports(CancellationToken cancellationToken)
        {
            var rpcResponse = await _productUserServiceClient.GetManufacturersAsync(new ManufacturersRequest());

            if (!rpcResponse.Success)
                throw new GRPCRequestFailException(rpcResponse.Message);

            var cutoffDate = DateTime.Now.ToUniversalTime().AddDays(-1);

            foreach(var manufacturerId in rpcResponse.ManufacturerId)
            {
                var manufacturerProducts = await _productRepository.GetManyProductsAsync(x => x.ManufacturerId.ToString() == manufacturerId, cancellationToken);

                var report = new Dictionary<Guid, int>();   

                foreach(var product in manufacturerProducts)
                {
                    var viewsLastDay = product.ViewAt.Count(view => view > cutoffDate);
                    report[product.Id] = viewsLastDay;
                }

                // TO DO
                //Send notification to each manufacturer
            }
        }
    }
}

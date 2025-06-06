using Microsoft.AspNetCore.Http.HttpResults;
using Proto.ProductUser;

namespace ProductService
{
    public class DailyProductsReportsGenerator
    {
        private const int REPORTS_TIME_INTERVAL = 24;

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
            Console.WriteLine("Daily report");
            var rpcResponse = await _productUserServiceClient.GetManufacturersAsync(new ManufacturersRequest());

            if (!rpcResponse.Success)
                throw new GRPCRequestFailException(rpcResponse.Message);

            var cutoffDate = DateTime.Now.ToUniversalTime().AddHours(-1 * REPORTS_TIME_INTERVAL);

            var dailyReportRpcRequest = new ManufacturersDailyReportRequest();

            foreach (var manufacturerId in rpcResponse.ManufacturerId)
            {
                var manufacturerDailyReport = new ManufacturerDailyReport()
                {
                    ManufacturerId = manufacturerId,
                };
                var manufacturerProducts = await _productRepository.GetManyProductsAsync(x => x.ManufacturerId.ToString() == manufacturerId, cancellationToken);

                foreach(var product in manufacturerProducts)
                {
                    var viewsLastDay = product.ViewAt.Count(view => view > cutoffDate);
                    var productViews = new ProductViews()
                    {
                        Key = product.Id.ToString(),
                        Value = viewsLastDay
                    };
                    manufacturerDailyReport.ProductsViews.Add(productViews);
                }
                
                dailyReportRpcRequest.Reports.Add(manufacturerDailyReport);
            }

            var dailyReportRpcResponse = await _productUserServiceClient.CreateManufacturersDailyReportAsync(dailyReportRpcRequest);

            if (!dailyReportRpcResponse.Success)
                throw new GRPCRequestFailException(dailyReportRpcResponse.Message);
        }
    }
}

using FluentAssertions;
using Moq;
using Proto.ProductUser;
using System.Linq.Expressions;

namespace ProductService
{
    public class DailyProductReportsGeneratorTests
    {
        private readonly Mock<ProductUserService.ProductUserServiceClient> _productUserServiceClientMock;
        private readonly Mock<IProductRepository> _productRepositoryMock;
        private readonly DailyProductsReportsGenerator _dailyReportsGenerator;

        public DailyProductReportsGeneratorTests()
        {
            _productUserServiceClientMock = new Mock<ProductUserService.ProductUserServiceClient>();
            _productRepositoryMock = new Mock<IProductRepository>();
            _dailyReportsGenerator = new DailyProductsReportsGenerator(
                _productUserServiceClientMock.Object,
                _productRepositoryMock.Object);
        }

        [Fact]
        public async Task GenerateDailyReports_ShouldThrowException_WhenRpcResponseIsNotSuccessful()
        {
            // Arrange
            var manufacturersResponse = new ManufacturerResponse
            {
                Success = false,
                Message = "Failed to get manufacturers",
                ManufacturerId = { }
            };

            var manufacturerMockCall = CallHelpers.CreateAsyncUnaryCall(manufacturersResponse);
            _productUserServiceClientMock
                .Setup(x => x.GetManufacturersAsync(
                    It.IsAny<ManufacturersRequest>(), null, null, CancellationToken.None))
                .Returns(manufacturerMockCall);

            var dailyReportResponse = new ProductResponse()
            {
                Success = true,
                Message = "Success test"
            };

            var dailyReportMockCall = CallHelpers.CreateAsyncUnaryCall(dailyReportResponse);
            _productUserServiceClientMock
                .Setup(x => x.CreateManufacturersDailyReportAsync(
                    It.IsAny<ManufacturersDailyReportRequest>(), null, null, CancellationToken.None))
                .Returns(dailyReportMockCall);

            var cancellationToken = new CancellationToken();

            // Act
            var act = async () => await _dailyReportsGenerator.GenerateDailyReports(cancellationToken);

            // Assert
            await act.Should().ThrowAsync<GRPCRequestFailException>()
                .WithMessage("Failed to get manufacturers");
        }

        [Fact]
        public async Task GenerateDailyReports_ShouldGenerateReports_WhenRpcResponseIsSuccessful()
        {
            // Arrange
            var manufacturerId = Guid.NewGuid().ToString();
            var manufacturersResponse = new ManufacturerResponse
            {
                Success = true,
                Message = "Manufacturers retrieved successfully",
                ManufacturerId = { manufacturerId }
            };

            var products = new List<Product>
            {
                new Product
                {
                    Id = Guid.NewGuid(),
                    ManufacturerId = Guid.Parse(manufacturerId),
                    ViewAt = new List<DateTime>
                    {
                        DateTime.Now.AddHours(-1), 
                        DateTime.Now.AddDays(-2)   
                    }
                }
            };

            var mockCall = CallHelpers.CreateAsyncUnaryCall(manufacturersResponse);
            _productUserServiceClientMock
                .Setup(x => x.GetManufacturersAsync(
                    It.IsAny<ManufacturersRequest>(), null, null, CancellationToken.None))
                .Returns(mockCall);

            var dailyReportResponse = new ProductResponse()
            {
                Success = true,
                Message = "Success test"
            };

            var dailyReportMockCall = CallHelpers.CreateAsyncUnaryCall(dailyReportResponse);
            _productUserServiceClientMock
                .Setup(x => x.CreateManufacturersDailyReportAsync(
                    It.IsAny<ManufacturersDailyReportRequest>(), null, null, CancellationToken.None))
                .Returns(dailyReportMockCall);

            _productRepositoryMock.Setup(x => x.GetManyProductsAsync(It.IsAny<Expression<Func<Product, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(products);

            var cancellationToken = new CancellationToken();

            // Act
            await _dailyReportsGenerator.GenerateDailyReports(cancellationToken);

            // Assert
            _productRepositoryMock.Verify(x => x.GetManyProductsAsync(It.IsAny<Expression<Func<Product, bool>>>(), cancellationToken), Times.Once);
        }
    }
}

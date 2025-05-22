
using MediatR;
using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;

namespace ProductService
{
    public class GetProductQueryHandler : IRequestHandler<GetProductQuery, Product>
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductRedisService _redisService;
        private readonly IBackgroundTaskService _backgroundTaskService;

        public GetProductQueryHandler(IProductRepository productRepository, 
                                      IProductRedisService redisService, 
                                      IBackgroundTaskService backgroundTaskService)
        {
            _productRepository = productRepository;
            _redisService = redisService;
            _backgroundTaskService = backgroundTaskService;
        }

        public async Task<Product> Handle(GetProductQuery request, CancellationToken cancellationToken)
        {
            var now = DateTime.Now;
            var lastDay = now.AddDays(-1);
            var lastWeek = now.AddDays(-7);

            if (_redisService.IsConnected())
            {
                var product = await _redisService.GetCacheValue(request.Id.ToString());
                if (product == null)
                {
                    product = await _productRepository.GetByIdAsync(request.Id, cancellationToken);
                    if (product == null)
                        throw new EntityNotFoundException(nameof(Product), request.Id);

                    product.ViewAt.Add(DateTime.Now.ToUniversalTime());

                    var lastDayViews = product.ViewAt.Count(date => date >= lastDay && date <= now);
                    var lastWeekViews = product.ViewAt.Count(date => date >= lastWeek && date <= now);

                    if(lastDayViews > 100 || lastWeekViews > 500)
                    {
                        await _redisService.SetCacheValue(request.Id.ToString(), product);
                    }

                    await _productRepository.UpdateAsync(product, cancellationToken);

                    return product;
                }
                else
                {
                    product.ViewAt.Add(DateTime.Now.ToUniversalTime());
                    _backgroundTaskService.Run(async () =>
                    {
                        var lastDayViews = product.ViewAt.Count(date => date >= lastDay && date <= now);
                        var lastWeekViews = product.ViewAt.Count(date => date >= lastWeek && date <= now);

                        if (lastDayViews < 100 && lastWeekViews < 500)
                        {
                            await _redisService.RemoveCacheValue(request.Id.ToString());
                        }
                        else
                        {
                            await _redisService.SetCacheValue(request.Id.ToString(), product);
                        }

                        await _productRepository.UpdateAsync(product, cancellationToken);
                    });

                    return product;
                }
            }
            else
            {
                var product = await _productRepository.GetByIdAsync(request.Id, cancellationToken);
                if (product == null)
                    throw new EntityNotFoundException(nameof(Product), request.Id);

                product.ViewAt.Add(DateTime.Now.ToUniversalTime());

                await _productRepository.UpdateAsync(product, cancellationToken);

                return product;
            }
        }
    }
}

using Microsoft.Extensions.Logging;

namespace ProductService
{
    public class BackgroundTaskService : IBackgroundTaskService
    {
        private readonly ILogger<BackgroundTaskService> _logger;

        public BackgroundTaskService(ILogger<BackgroundTaskService> logger)
        {
            _logger = logger;
        }

        public void Run(Func<Task> task)
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    await task();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ошибка в фоновой задаче");
                }
            });
        }
    }
}

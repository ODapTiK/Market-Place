using Serilog;
using Serilog.Exceptions;

namespace AuthorizationService
{
    public class LoggingService
    {
        public static void Configure(IConfiguration configuration)
        {
            var logstashUri = configuration["Logstash"];

            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithExceptionDetails()
                .WriteTo.Http(logstashUri!, queueLimitBytes: null)
                .CreateLogger();
        }
    }
}

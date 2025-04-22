using RabbitMQ.Client;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace ProductService
{
    public class RabbitMqInitializer : IHostedService
    {
        private readonly RabbitMqOptions _rabbitMqOptions;

        public RabbitMqInitializer(IOptions<RabbitMqOptions> options)
        {
            _rabbitMqOptions = options.Value;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var factory = new ConnectionFactory()
            {
                UserName = _rabbitMqOptions.UserName,
                Password = _rabbitMqOptions.Password,
                VirtualHost = _rabbitMqOptions.VirtualHost,
                HostName = _rabbitMqOptions.HostName,
                Port = _rabbitMqOptions.Port
            };

            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(
                queue: "UpdatedProducts",  
                durable: true,             
                exclusive: false,
                autoDelete: false,
                arguments: null);
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
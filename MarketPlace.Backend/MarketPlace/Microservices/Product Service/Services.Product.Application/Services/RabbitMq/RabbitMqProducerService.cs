using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace ProductService
{
    public class RabbitMqProducerService : IRabbitMqProducerService
    {
        private readonly IRabbitMqOptions _rabbitMqOptions;
        public RabbitMqProducerService(IOptions<IRabbitMqOptions> options)
        {
            _rabbitMqOptions = options.Value;
        }
        public async Task SendMessage(object message, string queueName)
        {
            await SendMessage(JsonSerializer.Serialize(message), queueName);
        }

        public async Task SendMessage(string message, string queueName)
        {
            var factory = new ConnectionFactory()
            {
                UserName = _rabbitMqOptions.UserName,
                Password = _rabbitMqOptions.Password,
                VirtualHost = _rabbitMqOptions.VirtualHost,
                HostName = _rabbitMqOptions.HostName,
                Port = _rabbitMqOptions.Port
            };
            using (var connection = await factory.CreateConnectionAsync())
            using (var channel = await connection.CreateChannelAsync())
            {
                await channel.QueueDeclareAsync(queue: queueName,
                               durable: false,
                               exclusive: false,
                               autoDelete: false,
                               arguments: null);

                var body = Encoding.UTF8.GetBytes(message);

                var props = new BasicProperties();

                await channel.BasicPublishAsync(exchange: "",
                               routingKey: queueName,
                               mandatory: false,
                               basicProperties: props,
                               body: body);
            }
        }
    }
}

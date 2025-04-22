using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace OrderService
{
    public class UpdateProductConsumer : BackgroundService
    {
        private readonly string _queueName;
        private readonly IChannel _channel;
        private readonly IServiceProvider _serviceProvider;
        private readonly RabbitMqOptions _rabbitMqOptions;

        public UpdateProductConsumer(IServiceProvider serviceProvider, IOptions<RabbitMqOptions> options)
        {
            _rabbitMqOptions = options.Value;
            _serviceProvider = serviceProvider;
            _queueName = "UpdatedProducts";
            var factory = new ConnectionFactory()
            {
                UserName = _rabbitMqOptions.UserName,
                Password = _rabbitMqOptions.Password,
                VirtualHost = _rabbitMqOptions.VirtualHost,
                HostName = _rabbitMqOptions.HostName,
                Port = _rabbitMqOptions.Port
            };
            var connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
            _channel = connection.CreateChannelAsync().GetAwaiter().GetResult();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (ch, ea) =>
            {
                var body = ea.Body.ToArray();

                var updatedProductId = Guid.Parse(Encoding.UTF8.GetString(body));
                using (var scope = _serviceProvider.CreateScope())
                {
                    var orderRepository = scope.ServiceProvider.GetRequiredService<IOrderRepository>();

                    var ordersWithUpdatedProducts = await orderRepository
                        .GetManyOrdersAsync(x => x.OrderPoints.Any(x => x.ProductId.Equals(updatedProductId)), stoppingToken);

                    var usersIdToWarn = ordersWithUpdatedProducts.Select(x => x.UserId).Distinct().ToList();

                    Console.WriteLine("message get");
                    //TO DO 
                    //Send message for each user whose order has been changed
                }
                await _channel.BasicAckAsync(ea.DeliveryTag, false);
            };
            string consumerTag = await _channel.BasicConsumeAsync(_queueName, false, consumer);
        }
    }
}

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
        private readonly IOrderRepository _orderRepository;
        private readonly IRabbitMqOptions _rabbitMqOptions;

        public UpdateProductConsumer(IOrderRepository orderRepository, IOptions<IRabbitMqOptions> options)
        {
            _rabbitMqOptions = options.Value;
            _orderRepository = orderRepository;
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
                var ordersWithUpdatedProducts = await _orderRepository
                    .GetManyOrdersAsync(x => x.OrderPoints.Any(x => x.ProductId.Equals(updatedProductId)), stoppingToken);

                var usersIdToWarn = ordersWithUpdatedProducts.Select(x => x.UserId).Distinct().ToList();

                //TO DO 
                //Send message for each user whose order has been changed

                await _channel.BasicAckAsync(ea.DeliveryTag, false);
            };
            string consumerTag = await _channel.BasicConsumeAsync(_queueName, false, consumer);
        }
    }
}

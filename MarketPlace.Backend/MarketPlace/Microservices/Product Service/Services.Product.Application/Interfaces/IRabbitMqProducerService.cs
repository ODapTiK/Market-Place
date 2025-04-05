namespace ProductService
{
    public interface IRabbitMqProducerService
    {
        public Task SendMessage(object message, string queueName);
        public Task SendMessage(string message, string queueName);
    }
}

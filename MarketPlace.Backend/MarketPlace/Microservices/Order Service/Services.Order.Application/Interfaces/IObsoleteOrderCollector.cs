namespace OrderService
{
    public interface IObsoleteOrderCollector
    {
        public Task RemoveObsoleteOrderAsync(Order order, CancellationToken cancellationToken);
    }
}

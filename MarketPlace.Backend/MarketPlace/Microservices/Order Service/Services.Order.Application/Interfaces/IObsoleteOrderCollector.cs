namespace OrderService
{
    public interface IObsoleteOrderCollector
    {
        public Task RemoveObsoleteOrder(Order order, CancellationToken cancellationToken);
    }
}

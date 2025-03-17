namespace OrderService
{
    public class OrderPoint
    {
        public Guid Id { get; set; }
        public Guid Orderid { get; set; }
        public Guid ProductId { get; set; }
        public int NumberOfUnits { get; set; }
        public Order? Order { get; set; }
    }
}

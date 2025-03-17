namespace OrderService
{
    public class Order
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public List<OrderPoint> OrderPoints { get; set; } = new List<OrderPoint>();
        public decimal TotalPrice { get; set; }
        public DateTime OrderDateTime { get; set; }
    }
}

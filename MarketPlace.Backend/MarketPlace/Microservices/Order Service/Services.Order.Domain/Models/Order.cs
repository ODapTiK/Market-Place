namespace OrderService
{
    public class Order
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public List<OrderPoint> OrderPoints { get; set; } = new List<OrderPoint>();
        public double TotalPrice { get; set; }
        public string Status { get; set; } = string.Empty;
        public Guid ControlAdminId { get; set; } 
        public DateTime OrderDateTime { get; set; }
    }
}

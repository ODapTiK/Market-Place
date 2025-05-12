namespace OrderService
{
    public class OrderPoint
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }
        public string? productName { get; set; } = null;
        public string? productDescription { get; set; } = null;
        public string? productCategory { get; set; } = null;
        public string? productType { get; set; } = null;
        public string? productImage { get; set; } = null;
        public int NumberOfUnits { get; set; }
    }
}

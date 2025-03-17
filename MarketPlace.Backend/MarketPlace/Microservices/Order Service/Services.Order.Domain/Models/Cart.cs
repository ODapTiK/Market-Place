namespace OrderService
{
    public class Cart
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public List<Guid> Products { get; set; } = [];
    }
}

namespace ProductService
{
    public class Review
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Description { get; set; } = string.Empty;
        public int Raiting { get; set; }
    }
}

namespace ProductService
{
    [Serializable]
    public class Product : Entity
    {
        public Guid ManufacturerId { get; set; }
        public string? Name { get; set; } 
        public string? Description { get; set; } 
        public string? Category { get; set; } 
        public string? Type { get; set; } 
        public List<Review> Reviews { get; set; } = [];
        public string? Image { get; set; } 
        public double Price { get; set; }
        public double Raiting { get; set; }
        public List<DateTime> ViewAt { get; set; } = [];

    }
}

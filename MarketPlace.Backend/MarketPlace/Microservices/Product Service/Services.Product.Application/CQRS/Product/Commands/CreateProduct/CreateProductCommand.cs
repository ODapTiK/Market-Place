using MediatR;

namespace ProductService
{
    public class CreateProductCommand : IRequest<Guid>
    {
        public Guid ManufacturerId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public string? Type { get; set; }
        public string? Image { get; set; }
        public double Price { get; set; }
    }
}

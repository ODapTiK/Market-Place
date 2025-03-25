using MediatR;

namespace ProductService
{
    public class UpdateProductCommand : IRequest
    {
        public Guid Id { get; set; }
        public Guid ManufacturerId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public string? Type { get; set; }
        public string? Image { get; set; }
        public decimal Price { get; set; }
    }
}

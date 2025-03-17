using MediatR;

namespace ProductService
{
    public class CreateProductReviewCommand : IRequest<Guid>
    {
        public Guid UserId { get; set; }
        public Guid ProductId { get; set; }
        public int Raiting { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}

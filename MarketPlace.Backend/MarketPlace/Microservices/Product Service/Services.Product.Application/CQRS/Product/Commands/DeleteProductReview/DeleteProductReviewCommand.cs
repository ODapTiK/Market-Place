using MediatR;

namespace ProductService
{
    public class DeleteProductReviewCommand : IRequest
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid ProductId { get; set; }
    }
}

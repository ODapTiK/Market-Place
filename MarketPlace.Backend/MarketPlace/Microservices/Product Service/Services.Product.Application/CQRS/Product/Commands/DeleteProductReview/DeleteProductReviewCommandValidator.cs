using FluentValidation;

namespace ProductService
{
    public class DeleteProductReviewCommandValidator : AbstractValidator<DeleteProductReviewCommand>
    {
        public DeleteProductReviewCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.ProductId).NotEmpty();
            RuleFor(x => x.UserId).NotEmpty();
        }
    }
}

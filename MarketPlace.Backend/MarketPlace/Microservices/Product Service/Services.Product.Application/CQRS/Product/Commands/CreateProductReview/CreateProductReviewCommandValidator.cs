using FluentValidation;

namespace ProductService
{
    public class CreateProductReviewCommandValidator : AbstractValidator<CreateProductReviewCommand>
    {
        public CreateProductReviewCommandValidator()
        {
            RuleFor(x => x.ProductId).NotEmpty();
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.Description).NotEmpty();
            RuleFor(x => x.Raiting).NotEmpty().InclusiveBetween(0, 5);
        }
    }
}

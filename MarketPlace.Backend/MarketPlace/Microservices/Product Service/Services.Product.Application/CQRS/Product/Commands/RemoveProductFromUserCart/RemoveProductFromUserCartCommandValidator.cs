using FluentValidation;

namespace ProductService
{
    public class RemoveProductFromUserCartCommandValidator : AbstractValidator<RemoveProductFromUserCartCommand>
    {
        public RemoveProductFromUserCartCommandValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.ProductId).NotEmpty();
        }
    }
}

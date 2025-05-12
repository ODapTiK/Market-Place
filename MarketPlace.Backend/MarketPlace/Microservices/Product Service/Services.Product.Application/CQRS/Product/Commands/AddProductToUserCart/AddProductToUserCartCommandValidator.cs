using FluentValidation;

namespace ProductService
{
    public class AddProductToUserCartCommandValidator : AbstractValidator<AddProductToUserCartCommand>
    {
        public AddProductToUserCartCommandValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.ProductId).NotEmpty();
        }
    }
}

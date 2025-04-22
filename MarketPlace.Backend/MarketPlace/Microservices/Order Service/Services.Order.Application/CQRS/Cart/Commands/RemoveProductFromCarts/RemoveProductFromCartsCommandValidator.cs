using FluentValidation;

namespace OrderService
{
    public class RemoveProductFromCartsCommandValidator : AbstractValidator<RemoveProductFromCartsCommand>
    {
        public RemoveProductFromCartsCommandValidator()
        {
            RuleFor(x => x.ProductId).NotEmpty();
        }
    }
}

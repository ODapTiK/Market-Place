using FluentValidation;

namespace OrderService
{
    public class RemoveOrderPointCommandValidator : AbstractValidator<RemoveOrderPointCommand>
    {
        public RemoveOrderPointCommandValidator()
        {
            RuleFor(x => x.ProductId).NotEmpty();
            RuleFor(x => x.CartId).NotEmpty();
        }
    }
}

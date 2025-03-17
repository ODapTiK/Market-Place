using FluentValidation;

namespace OrderService
{
    public class AddOrderPointCommandValidator : AbstractValidator<AddOrderPointCommand>
    {
        public AddOrderPointCommandValidator()
        {
            RuleFor(x => x.ProductId).NotEmpty();
            RuleFor(x => x.CartId).NotEmpty();
        }
    }
}

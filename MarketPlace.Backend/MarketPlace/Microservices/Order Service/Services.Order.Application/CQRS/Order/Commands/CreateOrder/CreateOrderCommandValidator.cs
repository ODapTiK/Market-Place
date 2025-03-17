using FluentValidation;

namespace OrderService
{
    public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
    {
        public CreateOrderCommandValidator()
        {
            RuleFor(x => x.TotalPrice).NotEmpty().Must(x => x >= 0);
            RuleFor(x => x.Points).NotEmpty().Must(x => x.TrueForAll(x => x.NumberOfUnits >= 0)).Must(x => x.TrueForAll(x => !x.ProductId.Equals(Guid.Empty)));
            RuleFor(x => x.UserId).NotEmpty();
        }
    }
}

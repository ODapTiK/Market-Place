using FluentValidation;

namespace OrderService
{
    public class SetOrderStatusReadyCommandValidator : AbstractValidator<SetOrderStatusReadyCommand>
    {
        public SetOrderStatusReadyCommandValidator()
        {
            RuleFor(x => x.AdminId).NotEmpty();
            RuleFor(x => x.OrderId).NotEmpty();
        }
    }
}

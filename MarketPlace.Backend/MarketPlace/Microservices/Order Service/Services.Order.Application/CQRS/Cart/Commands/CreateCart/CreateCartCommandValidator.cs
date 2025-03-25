using FluentValidation;

namespace OrderService
{
    public class CreateCartCommandValidator : AbstractValidator<CreateCartCommand>
    {
        public CreateCartCommandValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();
        }
    }
}

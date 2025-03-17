using FluentValidation;

namespace OrderService
{
    public class DeleteCartCommandValidator : AbstractValidator<DeleteCartCommand>
    {
        public DeleteCartCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}

using FluentValidation;

namespace OrderService
{
    public class DeleteOrderCommandValidator : AbstractValidator<DeleteOrderCommand>
    {
        public DeleteOrderCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}

using FluentValidation;

namespace OrderService
{
    public class DeleteUserOrdersCommandValidator : AbstractValidator<DeleteUserOrdersCommand>
    {
        public DeleteUserOrdersCommandValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();
        }
    }
}

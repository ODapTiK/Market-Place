using FluentValidation;

namespace OrderService
{
    public class GetUserCartQueryValidator : AbstractValidator<GetUserCartQuery>
    {
        public GetUserCartQueryValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();
        }
    }
}

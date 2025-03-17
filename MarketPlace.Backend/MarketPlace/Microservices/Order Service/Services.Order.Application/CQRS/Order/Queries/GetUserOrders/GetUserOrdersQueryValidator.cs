using FluentValidation;

namespace OrderService
{
    public class GetUserOrdersQueryValidator : AbstractValidator<GetUserOrdersQuery>
    {
        public GetUserOrdersQueryValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();
        }
    }
}

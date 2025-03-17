using FluentValidation;

namespace OrderService
{
    public class GetOrderQueryValidator : AbstractValidator<GetOrderQuery>
    {
        public GetOrderQueryValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}

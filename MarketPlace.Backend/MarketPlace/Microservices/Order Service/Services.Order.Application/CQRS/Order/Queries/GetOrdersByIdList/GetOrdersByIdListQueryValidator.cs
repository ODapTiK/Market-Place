using FluentValidation;

namespace OrderService
{
    public class GetOrdersByIdListQueryValidator : AbstractValidator<GetOrdersByIdListQuery>
    {
        public GetOrdersByIdListQueryValidator()
        {
            RuleFor(x => x.OrderIds).NotNull();
        }
    }
}

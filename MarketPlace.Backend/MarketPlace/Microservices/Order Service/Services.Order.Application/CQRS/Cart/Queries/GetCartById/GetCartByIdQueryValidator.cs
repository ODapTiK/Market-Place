using FluentValidation;

namespace OrderService
{
    public class GetCartByIdQueryValidator : AbstractValidator<GetCartByIdQuery>
    {
        public GetCartByIdQueryValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}

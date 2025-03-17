using FluentValidation;

namespace ProductService
{
    public class GetProductQueryValidator : AbstractValidator<GetProductQuery>
    {
        public GetProductQueryValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}

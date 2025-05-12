using FluentValidation;

namespace ProductService
{
    public class GetProductsByIdListQueryValidator : AbstractValidator<GetProductsByIdListQuery>
    {
        public GetProductsByIdListQueryValidator()
        {
            RuleFor(x => x.ProductIds).NotNull();
        }
    }
}

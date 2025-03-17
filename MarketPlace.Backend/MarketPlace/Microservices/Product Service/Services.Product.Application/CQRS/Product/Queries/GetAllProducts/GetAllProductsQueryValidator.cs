using FluentValidation;

namespace ProductService
{
    public class GetAllProductsQueryValidator : AbstractValidator<GetAllProductsQuery>
    {
        public GetAllProductsQueryValidator() { }
    }
}

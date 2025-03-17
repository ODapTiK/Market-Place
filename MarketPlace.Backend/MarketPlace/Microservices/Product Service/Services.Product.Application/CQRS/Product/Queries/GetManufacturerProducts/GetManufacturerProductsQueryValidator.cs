using FluentValidation;

namespace ProductService
{
    public class GetManufacturerProductsQueryValidator : AbstractValidator<GetManufacturerProductsQuery>
    {
        public GetManufacturerProductsQueryValidator()
        {
            RuleFor(x => x.ManufacturerId).NotEmpty();
        }
    }
}

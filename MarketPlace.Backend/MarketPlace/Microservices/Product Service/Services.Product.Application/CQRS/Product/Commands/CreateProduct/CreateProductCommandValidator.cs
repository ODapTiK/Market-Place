using FluentValidation;

namespace ProductService
{
    public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
    {
        public CreateProductCommandValidator()
        {
            RuleFor(x => x.ManufacturerId).NotEmpty();
            RuleFor(x => x.Price).NotEmpty().Must(x => x >= 0);
        }
    }
}

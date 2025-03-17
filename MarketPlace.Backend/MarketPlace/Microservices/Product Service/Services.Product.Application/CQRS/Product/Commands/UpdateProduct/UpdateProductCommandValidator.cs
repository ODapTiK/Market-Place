using FluentValidation;

namespace ProductService
{
    public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
    {
        public UpdateProductCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.ManufacturerId).NotEmpty();
            RuleFor(x => x.Price).NotEmpty().Must(x => x >= 0);
        }
    }
}

using FluentValidation;

namespace UserService
{
    public class ManufacturerDTOValidator : AbstractValidator<ManufacturerDTO>
    {
        public ManufacturerDTOValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Organization).NotEmpty().MaximumLength(150);
        }
    }
}

using FluentValidation;

namespace UserService
{
    public class AdminDTOValidator : AbstractValidator<AdminDTO>
    {
        public AdminDTOValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
            RuleFor(x => x.Surname).NotEmpty().MaximumLength(150);
        }
    }
}

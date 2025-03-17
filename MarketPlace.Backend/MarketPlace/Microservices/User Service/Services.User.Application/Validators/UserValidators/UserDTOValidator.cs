using FluentValidation;

namespace UserService
{
    public class UserDTOValidator : AbstractValidator<UserDTO> 
    {
        public UserDTOValidator()
        {
            RuleFor(x  => x.Id).NotEmpty();
            RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
            RuleFor(x => x.Surname).NotEmpty().MaximumLength(150);
            RuleFor(x => x.BirthDate).NotEmpty();
        }
    }
}

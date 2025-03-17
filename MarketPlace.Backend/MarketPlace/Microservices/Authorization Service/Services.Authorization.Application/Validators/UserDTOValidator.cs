using FluentValidation;

namespace AuthorizationService
{
    public class UserDTOValidator : AbstractValidator<UserDTO>
    {
        public UserDTOValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(500);
            RuleFor(x => x.Password).NotEmpty();
            RuleFor(x => x.Role).NotEmpty().Must(x => x.Equals("User") || x.Equals("Admin") || x.Equals("Manufacturer"));
        }
    }
}

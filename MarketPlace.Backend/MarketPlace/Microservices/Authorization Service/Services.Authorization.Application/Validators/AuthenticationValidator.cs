using FluentValidation;

namespace AuthorizationService
{
    public class AuthenticationValidator : AbstractValidator<AuthUserDTO>
    {
        public AuthenticationValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(500);
            RuleFor(x => x.Password).NotEmpty();
        }
    }
}

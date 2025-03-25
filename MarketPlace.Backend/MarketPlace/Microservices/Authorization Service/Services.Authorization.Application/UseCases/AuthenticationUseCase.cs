using FluentValidation;
using System.Security.Authentication;

namespace AuthorizationService
{
    public class AuthenticationUseCase : IAuthenticationUseCase
    {
        private readonly IUserRepository _userRepository;
        private readonly IValidator<AuthUserDTO> _validator;
        private readonly IPasswordEncryptor _passwordEncryptor;
        private readonly IJwtProvider _jwtProvider;

        public AuthenticationUseCase(IUserRepository userRepository,
                                     IValidator<AuthUserDTO> validator,
                                     IPasswordEncryptor passwordEncryptor,
                                     IJwtProvider jwtProvider)
        {
            _userRepository = userRepository;
            _validator = validator;
            _passwordEncryptor = passwordEncryptor;
            _jwtProvider = jwtProvider;
        }

        public async Task<TokenDTO> Execute(AuthUserDTO authUserDTO, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(authUserDTO);
            if (!validationResult.IsValid)
                throw new FluentValidation.ValidationException(validationResult.Errors);

            var user = await _userRepository.FindByEmailAsync(authUserDTO.Email, cancellationToken);

            if (user == null)
            {
                throw new EntityNotFoundException(nameof(User), authUserDTO.Email);
            }
            else if (_passwordEncryptor.VerifyPassword(user.PasswordHash, authUserDTO.Password))
            {
                return await _jwtProvider.GenerateToken(user, true, cancellationToken);
            }
            else throw new AuthenticationException();
        }
    }
}

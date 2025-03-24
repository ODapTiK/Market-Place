using FluentValidation;
using MediatR;
using System.Security.Authentication;

namespace AuthorizationService
{
    public class AuthenticationUseCase : IRequestHandler<AuthUserRequest, TokenDTO>
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

        public async Task<TokenDTO> Handle(AuthUserRequest request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request.authUserDTO);
            if (!validationResult.IsValid)
                throw new FluentValidation.ValidationException(validationResult.Errors);

            var user = await _userRepository.FindByEmailAsync(request.authUserDTO.Email, CancellationToken.None);

            if (user == null)
            {
                throw new EntityNotFoundException(nameof(User), request.authUserDTO.Email);
            }
            else if (_passwordEncryptor.VerifyPassword(user.Password, request.authUserDTO.Password))
            {
                return await _jwtProvider.GenerateToken(user, true, CancellationToken.None);
            }
            else throw new AuthenticationException();
        }
    }
}

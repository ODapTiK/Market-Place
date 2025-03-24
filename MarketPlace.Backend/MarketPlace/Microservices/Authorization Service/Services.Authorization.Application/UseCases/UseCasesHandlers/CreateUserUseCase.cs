using FluentValidation;
using MediatR;

namespace AuthorizationService
{
    public class CreateUserUseCase : IRequestHandler<CreateUserRequest, Guid>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordEncryptor _passwordEncryptor;
        private readonly IValidator<UserDTO> _validator;

        public CreateUserUseCase(IUserRepository userRepository, IPasswordEncryptor passwordEncryptor, IValidator<UserDTO> validator)
        {
            _userRepository = userRepository;
            _passwordEncryptor = passwordEncryptor;
            _validator = validator;
        }

        public async Task<Guid> Handle(CreateUserRequest request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request.userDTO);
            if (!validationResult.IsValid)
                throw new FluentValidation.ValidationException(validationResult.Errors);

            var userToCheck = await _userRepository.FindByEmailAsync(request.userDTO.Email, CancellationToken.None);
            if (userToCheck != null)
                throw new EntityAlreadyExistsException(nameof(User), request.userDTO.Email);

            var user = new User()
            {
                Id = Guid.NewGuid(),
                Email = request.userDTO.Email,
                Password = _passwordEncryptor.GenerateEncryptedPassword(request.userDTO.Password),
                Role = request.userDTO.Role,
            };

            return await _userRepository.CreateAsync(user, CancellationToken.None);
        }
    }
}

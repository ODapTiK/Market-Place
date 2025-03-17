using FluentValidation;

namespace AuthorizationService
{
    public class CreateUserUseCase : ICreateUserUseCase
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

        public async Task<Guid> Execute(UserDTO userDTO)
        {
            var validationResult = await _validator.ValidateAsync(userDTO);
            if (!validationResult.IsValid) 
                throw new FluentValidation.ValidationException(validationResult.Errors);    

            var user = new User()
            {
                Id = Guid.NewGuid(),
                Email = userDTO.Email,
                Password = _passwordEncryptor.GenerateEncryptedPassword(userDTO.Password),
                Role = userDTO.Role,
            };

            return await _userRepository.CreateAsync(user, CancellationToken.None);

        }
    }
}

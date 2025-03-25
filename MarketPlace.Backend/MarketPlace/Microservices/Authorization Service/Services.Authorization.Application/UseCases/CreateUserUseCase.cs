using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace AuthorizationService
{
    public class CreateUserUseCase : ICreateUserUseCase
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordEncryptor _passwordEncryptor;
        private readonly IValidator<UserDTO> _validator;
        private readonly IRoleRepository _roleRepository;

        public CreateUserUseCase(IUserRepository userRepository, IPasswordEncryptor passwordEncryptor, IValidator<UserDTO> validator, IRoleRepository roleRepository)
        {
            _userRepository = userRepository;
            _passwordEncryptor = passwordEncryptor;
            _validator = validator;
            _roleRepository = roleRepository;
        }

        public async Task<Guid> Execute(UserDTO userDTO, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(userDTO);
            if (!validationResult.IsValid) 
                throw new FluentValidation.ValidationException(validationResult.Errors);    

            var user = new User()
            {
                Id = Guid.NewGuid(),
                Email = userDTO.Email,
                PasswordHash = _passwordEncryptor.GenerateEncryptedPassword(userDTO.Password),
            };

            var userId = await _userRepository.CreateAsync(user, CancellationToken.None);

            if (!string.IsNullOrEmpty(userDTO.Role))
            {
                var roleExists = await _roleRepository.RoleExistsAsync(userDTO.Role);
                if (!roleExists)
                {
                    throw new EntityNotFoundException(nameof(IdentityRole), userDTO.Role);
                }

                var roleResult = await _userRepository.AddUserToRoleAsync(user, userDTO.Role);
                if (!roleResult.Succeeded)
                {
                    throw new Exception("Role assignment failed: " + string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                }
            }

            return user.Id;
        }
    }
}

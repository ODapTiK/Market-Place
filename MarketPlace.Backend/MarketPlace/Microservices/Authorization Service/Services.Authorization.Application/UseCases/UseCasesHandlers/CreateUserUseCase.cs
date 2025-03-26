using FluentValidation;
using Microsoft.AspNetCore.Identity;
using MediatR;

namespace AuthorizationService
{
    public class CreateUserUseCase : IRequestHandler<CreateUserRequest, Guid>
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
        public async Task<Guid> Handle(CreateUserRequest request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request.userDTO);
            if (!validationResult.IsValid)
                throw new FluentValidation.ValidationException(validationResult.Errors);

            var userToCheck = await _userRepository.FindByEmailAsync(request.userDTO.Email, cancellationToken);
            if (userToCheck != null)
                throw new EntityAlreadyExistsException(nameof(User), request.userDTO.Email);

            var user = new User()
            {
                Id = Guid.NewGuid(),
                Email = request.userDTO.Email,
                PasswordHash = _passwordEncryptor.GenerateEncryptedPassword(request.userDTO.Password),
            };

            var userId = await _userRepository.CreateAsync(user, cancellationToken);

            if (!string.IsNullOrEmpty(request.userDTO.Role))
            {
                var roleExists = await _roleRepository.RoleExistsAsync(request.userDTO.Role);
                if (!roleExists)
                {
                    throw new EntityNotFoundException(nameof(IdentityRole), request.userDTO.Role);
                }

                var roleResult = await _userRepository.AddUserToRoleAsync(user, request.userDTO.Role);
                if (!roleResult.Succeeded)
                {
                    throw new Exception("Role assignment failed: " + string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                }
            }

            return user.Id;
        }
    }
}

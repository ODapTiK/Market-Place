using FluentValidation;
using Microsoft.AspNetCore.Identity;
using MediatR;
using Proto.AuthUser;

namespace AuthorizationService
{
    public class CreateUserUseCase : IRequestHandler<CreateUserRequest, Guid>
    {
        private readonly IUserRepository _userRepository;
        private readonly IValidator<UserDTO> _validator;
        private readonly IRoleRepository _roleRepository;
        private readonly AuthUserService.AuthUserServiceClient _userServiceClient;

        public CreateUserUseCase(IUserRepository userRepository, 
                                 IValidator<UserDTO> validator, 
                                 IRoleRepository roleRepository,
                                 AuthUserService.AuthUserServiceClient userServiceClient)
        {
            _userRepository = userRepository;
            _validator = validator;
            _roleRepository = roleRepository;
            _userServiceClient = userServiceClient;
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
                UserName = request.userDTO.Email,
                PasswordHash = request.userDTO.Password
            };

            var userId = await _userRepository.CreateAsync(user, cancellationToken);

            if (!string.IsNullOrEmpty(request.userDTO.Role))
            {
                var roleExists = await _roleRepository.RoleExistsAsync(request.userDTO.Role);
                if (!roleExists)
                    throw new EntityNotFoundException(nameof(IdentityRole<Guid>), request.userDTO.Role);

                var roleResult = await _userRepository.AddUserToRoleAsync(user, request.userDTO.Role);
                if (!roleResult.Succeeded)
                    throw new Exception("Role assignment failed: " + string.Join(", ", roleResult.Errors.Select(e => e.Description)));
            }

            switch (request.userDTO.Role)
            {
                case nameof(Role.User):
                    var rpcUser = request.userDTO as CreateUserDTO;
                    if (rpcUser == null)
                        throw new ConversionException(nameof(UserDTO), nameof(CreateUserDTO));

                    var userRpcRequest = new Proto.AuthUser.CreateUserRequest
                    {
                        Id = userId.ToString(),
                        Name = rpcUser.Name,
                        Surname = rpcUser.Surname,
                        BirthDate = new Google.Protobuf.WellKnownTypes.Timestamp
                        {
                            Seconds = new DateTimeOffset(rpcUser.BirthDate).ToUnixTimeSeconds()
                        }
                    };

                    var userResponse = await _userServiceClient.CreateUserAsync(userRpcRequest);

                    if (!userResponse.Success)
                        throw new GRPCRequestFailException(userResponse.Message);
                    break;
                case nameof(Role.Admin):
                    var rpcAdmin = request.userDTO as CreateAdminDTO;
                    if (rpcAdmin == null)
                        throw new ConversionException(nameof(UserDTO), nameof(CreateAdminDTO));

                    var adminRpcRequest = new CreateAdminRequest
                    {
                        Id = userId.ToString(),
                        Name = rpcAdmin.Name,
                        Surname = rpcAdmin.Surname
                    };

                    var adminResponse = await _userServiceClient.CreateAdminAsync(adminRpcRequest);

                    if (!adminResponse.Success)
                        throw new GRPCRequestFailException(adminResponse.Message);
                    break;
                case nameof(Role.Manufacturer):
                    var rpcManufacturer = request.userDTO as CreateManufacturerDTO;
                    if (rpcManufacturer == null)
                        throw new ConversionException(nameof(UserDTO), nameof(CreateManufacturerDTO));

                    var manufacturerRpcRequest = new CreateManufacturerRequest
                    {
                        Id = userId.ToString(),
                        Organization = rpcManufacturer.Organization
                    };

                    var manufacturerResponse = await _userServiceClient.CreateManufacturerAsync(manufacturerRpcRequest);

                    if (!manufacturerResponse.Success)
                        throw new GRPCRequestFailException(manufacturerResponse.Message);
                    break;
                default:
                    throw new EntityNotFoundException(nameof(IdentityRole<Guid>), request.userDTO.Role);
            }

            return userId;
        }
    }
}

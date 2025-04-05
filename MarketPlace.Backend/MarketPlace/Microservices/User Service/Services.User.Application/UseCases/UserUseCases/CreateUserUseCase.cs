using AutoMapper;
using FluentValidation;
using Proto.OrderUser;

namespace UserService
{
    public class CreateUserUseCase : ICreateUserUseCase
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly IValidator<UserDTO> _validator;
        private readonly OrderUserService.OrderUserServiceClient _userServiceClient;
        public CreateUserUseCase(IMapper mapper, 
                                 IUserRepository userRepository, 
                                 IValidator<UserDTO> validator,
                                 OrderUserService.OrderUserServiceClient userServiceClient)
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _validator = validator;
            _userServiceClient = userServiceClient;
        }

        public async Task<Guid> Execute(UserDTO userDTO, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(userDTO);
            if (!validationResult.IsValid)
            {
                throw new FluentValidation.ValidationException(validationResult.Errors);
            }

            var userId = await _userRepository.CreateAsync(_mapper.Map<User>(userDTO), cancellationToken);

            var rpcRequest = new CartRequest
            {
                UserId = userId.ToString()
            };

            var rpcResponse = await _userServiceClient.CreateCartAsync(rpcRequest);

            if(!rpcResponse.Success)
                throw new GRPCRequestFailException(rpcResponse.Message);

            return userId;
        }
    }
}

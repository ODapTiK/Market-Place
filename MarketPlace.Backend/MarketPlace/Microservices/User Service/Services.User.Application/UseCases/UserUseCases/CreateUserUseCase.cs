using AutoMapper;
using FluentValidation;

namespace UserService
{
    public class CreateUserUseCase : ICreateUserUseCase
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly IValidator<UserDTO> _validator;
        public CreateUserUseCase(IMapper mapper, IUserRepository userRepository, IValidator<UserDTO> validator)
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _validator = validator;
        }

        public async Task<Guid> Execute(UserDTO userDTO)
        {
            var validationResult = await _validator.ValidateAsync(userDTO);
            if (!validationResult.IsValid)
            {
                throw new FluentValidation.ValidationException(validationResult.Errors);
            }

            return await _userRepository.CreateAsync(_mapper.Map<User>(userDTO), CancellationToken.None);
        }
    }
}

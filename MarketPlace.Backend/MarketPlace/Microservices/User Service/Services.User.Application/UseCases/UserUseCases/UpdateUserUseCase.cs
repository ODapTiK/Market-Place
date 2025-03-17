using AutoMapper;
using FluentValidation;

namespace UserService
{
    public class UpdateUserUseCase : IUpdateUserUseCase
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IValidator<UserDTO> _validator;

        public UpdateUserUseCase(IUserRepository userRepository, IMapper mapper, IValidator<UserDTO> validator)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _validator = validator;
        }

        public async Task Execute(UserDTO userDTO)
        {
            var validationResult = await _validator.ValidateAsync(userDTO);
            if (!validationResult.IsValid)
            {
                throw new FluentValidation.ValidationException(validationResult.Errors);
            }

            var user = await _userRepository.GetByIdAsync(userDTO.Id, CancellationToken.None);

            if (user == null) 
                throw new EntityNotFoundException(nameof(User), userDTO.Id);

            user = _mapper.Map<User>(userDTO);

            await _userRepository.UpdateAsync(CancellationToken.None);
        }
    }
}

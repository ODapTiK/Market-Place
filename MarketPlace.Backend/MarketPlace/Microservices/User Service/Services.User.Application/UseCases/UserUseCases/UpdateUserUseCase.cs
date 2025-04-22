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

        public async Task Execute(UserDTO userDTO, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(userDTO);
            if (!validationResult.IsValid)
            {
                throw new FluentValidation.ValidationException(validationResult.Errors);
            }

            var user = await _userRepository.GetByIdAsync(userDTO.Id, cancellationToken);

            if (user == null) 
                throw new EntityNotFoundException(nameof(User), userDTO.Id);

            user.BirthDate = userDTO.BirthDate;
            user.Name = userDTO.Name;
            user.Surname = userDTO.Surname;

            await _userRepository.UpdateAsync(cancellationToken);
        }
    }
}

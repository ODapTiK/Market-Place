using AutoMapper;
using FluentValidation;

namespace UserService
{
    public class CreateAdminUseCase : ICreateAdminUseCase
    {
        private readonly IAdminRepository _adminRepository;
        private readonly IMapper _mapper;
        private readonly IValidator<AdminDTO> _validator;

        public CreateAdminUseCase(IAdminRepository adminRepository, IMapper mapper, IValidator<AdminDTO> validator)
        {
            _adminRepository = adminRepository;
            _mapper = mapper;
            _validator = validator;
        }

        public async Task<Guid> Execute(AdminDTO adminDTO, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(adminDTO);
            if (!validationResult.IsValid)
                throw new FluentValidation.ValidationException(validationResult.Errors);

            return await _adminRepository.CreateAsync(_mapper.Map<Admin>(adminDTO), cancellationToken);
        }
    }
}

using AutoMapper;
using FluentValidation;

namespace UserService
{
    public class UpdateAdminUseCase : IUpdateAdminUseCase
    {
        private readonly IAdminRepository _adminRepository;
        private readonly IMapper _mapper;
        private readonly IValidator<AdminDTO> _validator;

        public UpdateAdminUseCase(IAdminRepository adminRepository, IMapper mapper, IValidator<AdminDTO> validator)
        {
            _adminRepository = adminRepository;
            _mapper = mapper;
            _validator = validator;
        }

        public async Task Execute(AdminDTO adminDTO, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(adminDTO);
            if (!validationResult.IsValid)
                throw new FluentValidation.ValidationException(validationResult.Errors);

            var admin = await _adminRepository.GetByIdAsync(adminDTO.Id, cancellationToken);
            if (admin == null)
                throw new EntityNotFoundException(nameof(AdminDTO), adminDTO.Id);

            admin.Name = adminDTO.Name;
            admin.Surname = adminDTO.Surname;

            await _adminRepository.UpdateAsync(cancellationToken);
        }
    }
}

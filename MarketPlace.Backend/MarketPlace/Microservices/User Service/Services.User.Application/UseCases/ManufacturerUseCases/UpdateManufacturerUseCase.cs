using AutoMapper;
using FluentValidation;

namespace UserService
{
    public class UpdateManufacturerUseCase : IUpdateManufacturerUseCase
    {
        private readonly IManufacturerRepository _manufacturerRepository;
        private readonly IMapper _mapper;
        private readonly IValidator<ManufacturerDTO> _validator;

        public UpdateManufacturerUseCase(IManufacturerRepository manufacturerRepository, IMapper mapper, IValidator<ManufacturerDTO> validator)
        {
            _manufacturerRepository = manufacturerRepository;
            _mapper = mapper;
            _validator = validator;
        }

        public async Task Execute(ManufacturerDTO manufacturerDTO, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(manufacturerDTO);
            if (!validationResult.IsValid)
                throw new FluentValidation.ValidationException(validationResult.Errors);

            var manufacturer = await _manufacturerRepository.GetByIdAsync(manufacturerDTO.Id, cancellationToken);
            if(manufacturer == null)
                throw new EntityNotFoundException(nameof(Manufacturer), manufacturerDTO.Id);

            manufacturer = _mapper.Map<Manufacturer>(manufacturerDTO);

            await _manufacturerRepository.UpdateAsync(cancellationToken);
        }
    }
}

using AutoMapper;
using FluentValidation;

namespace UserService
{
    public class CreateManufacturerUseCase : ICreateManufacturerUseCase
    {
        private readonly IManufacturerRepository _manufacturerRepository;
        private readonly IMapper _mapper;
        private readonly IValidator<ManufacturerDTO> _validator;

        public CreateManufacturerUseCase(IManufacturerRepository manufacturerRepository, IMapper mapper, IValidator<ManufacturerDTO> validator)
        {
            _manufacturerRepository = manufacturerRepository;
            _mapper = mapper;
            _validator = validator;
        }

        public async Task<Guid> Execute(ManufacturerDTO manufacturerDTO, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(manufacturerDTO);
            if (!validationResult.IsValid)
                throw new FluentValidation.ValidationException(validationResult.Errors);

            return await _manufacturerRepository.CreateAsync(_mapper.Map<Manufacturer>(manufacturerDTO), cancellationToken);
        }
    }
}

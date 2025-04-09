namespace UserService
{
    public class GetManufacturersIdUseCase : IGetManufacturersIdUseCase
    {
        private readonly IManufacturerRepository _manufacturerRepository;

        public GetManufacturersIdUseCase(IManufacturerRepository manufacturerRepository)
        {
            _manufacturerRepository = manufacturerRepository;
        }

        public async Task<List<Guid>> Execute(CancellationToken cancellationToken)
        {
            var manufacturers = await _manufacturerRepository.GetAllAsync(cancellationToken);

            return manufacturers.Select(x => x.Id).ToList();
        }
    }
}

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
            return (await _manufacturerRepository.GetAllAsync(cancellationToken)).Select(x => x.Id).ToList();
        }
    }
}

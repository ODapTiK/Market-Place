namespace UserService
{
    public interface ICreateManufacturerUseCase
    {
        public Task<Guid> Execute(ManufacturerDTO manufacturerDTO, CancellationToken cancellationToken);
    }
}

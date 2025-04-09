namespace UserService
{
    public interface IGetManufacturersIdUseCase
    {
        public Task<List<Guid>> Execute(CancellationToken cancellationToken);
    }
}

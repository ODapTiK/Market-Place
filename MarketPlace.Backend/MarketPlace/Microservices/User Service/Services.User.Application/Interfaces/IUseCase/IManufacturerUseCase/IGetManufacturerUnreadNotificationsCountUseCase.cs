namespace UserService
{
    public interface IGetManufacturerUnreadNotificationsCountUseCase
    {
        public Task<int> Execute(Guid manufacturerId, CancellationToken cancellationToken);
    }
}

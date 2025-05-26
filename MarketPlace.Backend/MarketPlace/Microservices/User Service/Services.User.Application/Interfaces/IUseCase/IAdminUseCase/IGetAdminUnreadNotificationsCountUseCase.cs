namespace UserService
{
    public interface IGetAdminUnreadNotificationsCountUseCase
    {
        public Task<int> Execute(Guid adminId, CancellationToken cancellationToken);
    }
}

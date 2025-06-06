namespace UserService
{
    public interface IGetUserUnreadNotificationsCountUseCase
    {
        public Task<int> Execute(Guid userId, CancellationToken cancellationToken);
    }
}

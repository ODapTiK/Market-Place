namespace UserService
{
    public interface IGetUsersWithBirthdayUseCase
    {
        public Task<List<User>> Execute(CancellationToken cancellationToken);
    }
}

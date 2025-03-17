namespace UserService
{
    public interface IGetUserInfoUseCase
    {
        public Task<User> Execute(Guid userId);
    }
}

namespace UserService
{
    public interface IGetAdminInfoUseCase
    {
        public Task<Admin> Execute(Guid adminId);
    }
}

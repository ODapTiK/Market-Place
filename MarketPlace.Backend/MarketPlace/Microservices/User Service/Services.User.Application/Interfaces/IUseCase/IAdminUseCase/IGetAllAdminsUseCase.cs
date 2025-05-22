namespace UserService
{
    public interface IGetAllAdminsUseCase
    {
        public Task<List<Admin>> Execute(CancellationToken cancellationToken);
    }
}

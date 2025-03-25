namespace UserService
{
    public interface IDeleteAdminUseCase
    {
        public Task Execute(Guid adminId, CancellationToken cancellationToken);
    }
}

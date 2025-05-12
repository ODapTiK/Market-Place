namespace UserService
{
    public interface IRemoveControlAdminOrderUseCase
    {
        public Task Execute(Guid adminId, Guid orderId, CancellationToken cancellationToken);
    }
}

namespace UserService
{
    public interface IAddOrderToControlAdminUseCase
    {
        public Task Execute(Guid adminId, Guid orderId, CancellationToken cancellationToken);
    }
}

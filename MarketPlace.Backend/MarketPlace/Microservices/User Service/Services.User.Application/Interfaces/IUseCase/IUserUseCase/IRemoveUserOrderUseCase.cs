namespace UserService
{
    public interface IRemoveUserOrderUseCase
    {
        public Task Execute(Guid userId, Guid orderId);
    }
}

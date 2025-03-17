namespace UserService
{
    public interface IAddUserOrderUseCase
    {
        public Task Execute(Guid userId, Guid orderId);
    }
}

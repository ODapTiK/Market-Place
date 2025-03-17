namespace UserService
{
    public interface IDeleteUserUseCase
    {
        public Task Execute(Guid userId);
    }
}

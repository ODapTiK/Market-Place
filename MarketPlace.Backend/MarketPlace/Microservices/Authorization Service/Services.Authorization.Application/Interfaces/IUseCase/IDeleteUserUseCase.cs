namespace AuthorizationService
{
    public interface IDeleteUserUseCase
    {
        public Task Execute(Guid userId, CancellationToken cancellationToken);
    }
}

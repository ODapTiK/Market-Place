namespace AuthorizationService
{
    public interface ICreateUserUseCase
    {
        public Task<Guid> Execute(UserDTO userDTO, CancellationToken cancellationToken);
    }
}

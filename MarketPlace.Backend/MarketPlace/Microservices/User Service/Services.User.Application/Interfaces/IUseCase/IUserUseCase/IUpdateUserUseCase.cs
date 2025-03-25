namespace UserService
{
    public interface IUpdateUserUseCase
    {
        public Task Execute(UserDTO userDTO, CancellationToken cancellationToken);
    }
}

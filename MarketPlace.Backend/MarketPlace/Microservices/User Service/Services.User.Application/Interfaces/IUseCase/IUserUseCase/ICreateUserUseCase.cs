namespace UserService
{
    public interface ICreateUserUseCase
    {
        public Task<Guid> Execute(UserDTO userDTO);
    }
}

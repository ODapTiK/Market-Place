namespace UserService
{
    public interface IUpdateAdminUseCase
    {
        public Task Execute(AdminDTO adminDTO);
    }
}

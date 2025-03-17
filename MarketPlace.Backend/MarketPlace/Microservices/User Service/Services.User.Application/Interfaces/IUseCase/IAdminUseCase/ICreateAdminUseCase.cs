namespace UserService
{
    public interface ICreateAdminUseCase
    {
        public Task<Guid> Execute(AdminDTO adminDTO);
    }
}

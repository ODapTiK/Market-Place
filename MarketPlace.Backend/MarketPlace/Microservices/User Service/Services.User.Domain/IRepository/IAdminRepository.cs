namespace UserService
{
    public interface IAdminRepository : IBaseRepository<Admin>
    {
        public Task<List<Admin>> GetAllAsync(CancellationToken cancellationToken);
    }
}

namespace UserService
{
    public class AdminRepository : BaseRepository<Admin>, IAdminRepository
    {
        public AdminRepository(IUserDbContext userDbContext) : base(userDbContext) { }
    }
}

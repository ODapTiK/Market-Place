namespace UserService
{
    public class TestBaseRepository : BaseRepository<Admin>
    {
        public  TestBaseRepository(IUserDbContext context) : base(context) { }
    }
}

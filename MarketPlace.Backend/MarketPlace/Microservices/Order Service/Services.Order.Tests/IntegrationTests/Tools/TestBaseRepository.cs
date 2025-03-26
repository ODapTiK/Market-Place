namespace OrderService
{
    public class TestBaseRepository : BaseRepository<Order>
    {
        public TestBaseRepository(IOrderDbContext context) : base(context) { }
    }
}

namespace ProductService
{
    public class TestBaseRepository : BaseRepository<Product>
    {
        public TestBaseRepository(IProductDbContext context) : base(context, "TestEntities") { }
    }
}

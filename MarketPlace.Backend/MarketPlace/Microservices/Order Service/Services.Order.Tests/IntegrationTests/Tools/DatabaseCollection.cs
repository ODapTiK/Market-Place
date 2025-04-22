namespace OrderService
{
    [CollectionDefinition("Database Collection", DisableParallelization = true)]
    public class DatabaseCollection : ICollectionFixture<TestOrderDatabaseFixture>
    {
    }
}

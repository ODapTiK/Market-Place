namespace UserService
{
    [CollectionDefinition("Database Collection", DisableParallelization = true)]
    public class DatabaseCollection : ICollectionFixture<TestUserDatabaseFixture>
    {
    }
}

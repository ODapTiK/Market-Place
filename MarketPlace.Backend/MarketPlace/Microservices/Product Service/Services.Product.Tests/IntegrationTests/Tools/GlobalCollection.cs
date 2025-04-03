namespace ProductService
{
    [CollectionDefinition("Global Collection", DisableParallelization = true)]
    public class GlobalCollection : ICollectionFixture<BsonSerializerInitializer>
    {
        
    }
}

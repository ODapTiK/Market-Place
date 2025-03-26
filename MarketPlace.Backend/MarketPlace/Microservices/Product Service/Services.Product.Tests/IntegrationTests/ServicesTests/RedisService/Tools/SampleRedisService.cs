using StackExchange.Redis;

namespace ProductService
{
    public class SampleRedisService : BaseRedisService<SampleEntity>
    {
        public SampleRedisService(IConnectionMultiplexer connectionMultiplexer, int databaseIndex) 
            : base(connectionMultiplexer, databaseIndex) { }
    }
}

using MongoDB.Bson.Serialization.Attributes;

namespace ProductService
{
    public class Entity
    {
        [BsonId]
        public Guid Id { get; set; }
    }
}

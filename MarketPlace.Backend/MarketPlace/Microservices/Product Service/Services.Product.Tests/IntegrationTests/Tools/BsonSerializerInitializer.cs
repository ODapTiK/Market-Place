using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization;
using MongoDB.Bson;

namespace ProductService
{
    public class BsonSerializerInitializer
    {
        static BsonSerializerInitializer()
        {
            BsonSerializer.RegisterSerializer(typeof(Guid), new GuidSerializer(GuidRepresentation.Standard));
        }
    }
}

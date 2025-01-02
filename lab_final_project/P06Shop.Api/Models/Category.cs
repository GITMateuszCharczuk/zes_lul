using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace P06Shop.Api.Models
{
    public class Category
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
        public required string Name { get; set; }
        public required List<string> ProductIds { get; set; } = new List<string>();
    }
}
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace P06Shop.Api.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public required string Username { get; set; }
        public required string Email { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string PasswordHash { get; set; }
        public string Role { get; set; } = "Customer";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public List<string> OrderIds { get; set; } = new List<string>();
    }
} 
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace P06Shop.Api.Models
{
    public class Order
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        [BsonRepresentation(BsonType.ObjectId)]
        public required string UserId { get; set; }
        
        public required List<OrderItem> Items { get; set; } = new List<OrderItem>();
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public decimal TotalAmount { get; set; }
        public required string Status { get; set; } = "Pending";
    }

    public class OrderItem
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public required string ProductId { get; set; }
        public required int Quantity { get; set; }
        public required decimal PriceAtOrder { get; set; }
    }
} 
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace P06Shop.Api.Models.Dto
{
    public class GetOrderDTO
    {
        public required string Id { get; set; }
        public required string UserId { get; set; }
        public required List<GetOrderItemDTO> Items { get; set; } = new List<GetOrderItemDTO>();
        public required DateTime OrderDate { get; set; }
        public required decimal TotalAmount { get; set; }
        public required string Status { get; set; }
    }

    public class GetOrderItemDTO
    {
        public required string ProductId { get; set; }
        public required int Quantity { get; set; }
        public required decimal PriceAtOrder { get; set; }
    }
} 
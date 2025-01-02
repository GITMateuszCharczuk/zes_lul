namespace P06Shop.Api.Models.Dto
{
    public class CreateOrderDTO
    {
        public string? UserId { get; set; }
        public required List<CreateOrderItemDTO> Items { get; set; } = new List<CreateOrderItemDTO>();
    }

    public class CreateOrderItemDTO
    {
        public required string ProductId { get; set; }
        public required int Quantity { get; set; }
    }
}

namespace P06Shop.Api.Models.Dto
{
    public class UpdateOrderDTO
    {
        public string? Status { get; set; }
        public List<UpdateOrderItemDTO> Items { get; set; } = new List<UpdateOrderItemDTO>();
    }

    public class UpdateOrderItemDTO
    {
        public required string ProductId { get; set; }
        public required int Quantity { get; set; }
    }
}

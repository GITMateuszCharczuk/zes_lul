namespace P06Shop.Api.Models.Dto
{
    public class CreateProductDetailDTO
    {
        public int ProductId { get; set; } // Foreign key to Product
        public string Specifications { get; set; }
        public string Warranty { get; set; }
    }
} 
namespace P06Shop.Api.Models
{
    public class ProductDetail
    {
        public int Id { get; set; }
        public int ProductId { get; set; } // Foreign key to Product
        public string Specifications { get; set; }
        public string Warranty { get; set; }
    }
}
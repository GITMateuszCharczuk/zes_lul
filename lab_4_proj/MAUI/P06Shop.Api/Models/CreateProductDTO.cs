namespace P06Shop.Api.Models
{
    public class CreateProductDTO
    {
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required string Barcode { get; set; }
        public double Price { get; set; }
        public DateTime ReleaseDate { get; set; }
    }
}

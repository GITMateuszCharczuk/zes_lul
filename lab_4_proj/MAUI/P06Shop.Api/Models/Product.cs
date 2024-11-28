namespace P06Shop.Api.Models
{
    public class Product
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Barcode { get; set; }
        public double Price { get; set; }
        public DateTime ReleaseDate { get; set; }
    }
}

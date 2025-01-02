namespace P06Shop.Api.Models.Dto
{
    public class UpdateProductDTO
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public string? Barcode { get; set; }
        public double? Price { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public List<string>? Categories { get; set; } = new List<string>();
    }
}

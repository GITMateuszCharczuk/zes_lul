namespace P06Shop.Api.Models.Dto
{
    public class CreateCategoryDTO
    {
        public required string Name { get; set; }
        public List<int> ProductIds { get; set; } = new List<int>();
    }
} 
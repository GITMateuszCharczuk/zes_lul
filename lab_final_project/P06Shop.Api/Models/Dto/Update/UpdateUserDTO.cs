using MongoDB.Bson;

namespace P06Shop.Api.Models.Dto
{
    public class UpdateUserDTO
    {
        public string? Username { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
} 
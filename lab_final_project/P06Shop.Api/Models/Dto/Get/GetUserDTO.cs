using MongoDB.Bson;

namespace P06Shop.Api.Models.Dto
{
    public class GetUserDTO
    {
        public required string Id { get; set; }
        public required string Username { get; set; }
        public required string Email { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public List<string> OrderIds { get; set; } = new List<string>();
    }

    public class GetUsersDTO
    {
        public List<GetUserDTO> Users { get; set; } = new List<GetUserDTO>();
    }
} 
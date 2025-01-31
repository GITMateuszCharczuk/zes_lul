using MongoDB.Bson;

namespace P06Shop.Api.Models.Dto
{
    public class CreateUserDTO
    {
        public required string Username { get; set; }
        public required string Email { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Password { get; set; }
    }
} 
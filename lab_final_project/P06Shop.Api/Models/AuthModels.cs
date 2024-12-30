namespace P06Shop.Api.Models
{
    public class LoginModel
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }

    public class RegisterModel
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string Username { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
    }

    public class AuthResponse
    {
        public string Token { get; set; }
        public string UserId { get; set; }
        public string Username { get; set; }
        public string Role { get; set; }
    }
} 
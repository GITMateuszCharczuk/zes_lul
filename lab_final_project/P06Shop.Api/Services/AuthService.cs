using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using P06Shop.Api.Data;
using P06Shop.Api.Models;

namespace P06Shop.Api.Services
{
    public class AuthService
    {
        private readonly MongoDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(MongoDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<AuthResponse?> Login(LoginModel model)
        {
            var user = await _context.Users.Find(x => x.Email == model.Email).FirstOrDefaultAsync();
            if (user == null)
                return null;

            if (!VerifyPasswordHash(model.Password, user.PasswordHash))
                return null;

            var token = GenerateJwtToken(user);
            return new AuthResponse
            {
                Token = token,
                UserId = user.Id,
                Username = user.Username,
                Role = user.Role
            };
        }

        public async Task<AuthResponse?> Register(RegisterModel model)
        {
            if (await _context.Users.Find(x => x.Email == model.Email).AnyAsync())
                return null;

            if (await _context.Users.Find(x => x.Username == model.Username).AnyAsync())
                return null;

            // Check if this is the first user - if so, make them an admin
            bool isFirstUser = !await _context.Users.Find(_ => true).AnyAsync();

            var user = new User
            {
                Email = model.Email,
                Username = model.Username,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PasswordHash = HashPassword(model.Password),
                Role = isFirstUser ? "Admin" : "Customer"
            };

            await _context.Users.InsertOneAsync(user);

            var token = GenerateJwtToken(user);
            return new AuthResponse
            {
                Token = token,
                UserId = user.Id,
                Username = user.Username,
                Role = user.Role
            };
        }

        public async Task<bool> PromoteToAdmin(string userId)
        {
            var update = Builders<User>.Update.Set(u => u.Role, "Admin");
            var result = await _context.Users.UpdateOneAsync(u => u.Id == userId, update);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> DemoteToCustomer(string userId)
        {
            // Ensure we can't demote the last admin
            var adminCount = await _context.Users.CountDocumentsAsync(u => u.Role == "Admin");
            if (adminCount <= 1)
            {
                var userToCheck = await _context.Users.Find(u => u.Id == userId).FirstOrDefaultAsync();
                if (userToCheck?.Role == "Admin")
                    return false; // Can't demote the last admin
            }

            var update = Builders<User>.Update.Set(u => u.Role, "Customer");
            var result = await _context.Users.UpdateOneAsync(u => u.Id == userId, update);
            return result.ModifiedCount > 0;
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured"));
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }

        private static bool VerifyPasswordHash(string password, string storedHash)
        {
            var hashOfInput = HashPassword(password);
            return storedHash == hashOfInput;
        }
    }
} 
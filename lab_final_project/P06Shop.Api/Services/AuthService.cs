using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using MongoDB.Driver;
using P06Shop.Api.Data;
using P06Shop.Api.Models;

namespace P06Shop.Api.Services
{
    public class AuthService
    {
        private readonly MongoDbContext _context;
        private readonly JwtService _jwtService;

        public AuthService(MongoDbContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public async Task<AuthResponse> Login(string username, string password)
        {
            var user = await _context.Users.Find(u => u.Username == username).FirstOrDefaultAsync();
            if (user == null)
                return new AuthResponse { Success = false, Message = "User not found." };

            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return new AuthResponse { Success = false, Message = "Invalid password." };

            var token = _jwtService.GenerateToken(user);
            return new AuthResponse
            {
                Success = true,
                Token = token,
                Message = "User logged in successfully",
                UserId = user.Id,
                Username = user.Username,
                Role = user.Role
            };
        }

        public async Task<AuthResponse> Register(string username, string email, string password, string firstName, string lastName)
        {
            var existingUser = await _context.Users.Find(u => u.Username == username || u.Email == email).FirstOrDefaultAsync();
            if (existingUser != null)
                return new AuthResponse { Success = false, Message = "Username or email already exists." };

            var passwordHash = HashPassword(password);
            var user = new User
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Username = username,
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                PasswordHash = passwordHash,
                Role = "Customer",
                CreatedAt = DateTime.UtcNow,
                OrderIds = new List<string>()
            };

            await _context.Users.InsertOneAsync(user);

            var token = _jwtService.GenerateToken(user);
            return new AuthResponse
            {
                Success = true,
                Token = token,
                Message = "User registered successfully",
                UserId = user.Id,
                Username = user.Username,
                Role = user.Role
            };
        }

        public async Task<AuthResponse> PromoteToAdmin(string userId)
        {
            var user = await _context.Users.Find(u => u.Id == userId).FirstOrDefaultAsync();
            if (user == null)
                return new AuthResponse { Success = false, Message = "User not found." };

            var update = Builders<User>.Update.Set(u => u.Role, "Admin");
            await _context.Users.UpdateOneAsync(u => u.Id == userId, update);

            user.Role = "Admin";
            var token = _jwtService.GenerateToken(user);
            return new AuthResponse
            {
                Success = true,
                Token = token,
                Message = "User promoted to Admin",
                UserId = user.Id,
                Username = user.Username,
                Role = user.Role
            };
        }

        public async Task<AuthResponse> DemoteToCustomer(string userId)
        {
            var user = await _context.Users.Find(u => u.Id == userId).FirstOrDefaultAsync();
            if (user == null)
                return new AuthResponse { Success = false, Message = "User not found." };

            var update = Builders<User>.Update.Set(u => u.Role, "Customer");
            await _context.Users.UpdateOneAsync(u => u.Id == userId, update);

            user.Role = "Customer";
            var token = _jwtService.GenerateToken(user);
            return new AuthResponse
            {
                Success = true,
                Token = token,
                Message = "User demoted to Customer",
                UserId = user.Id,
                Username = user.Username,
                Role = user.Role
            };
        }
    }
} 
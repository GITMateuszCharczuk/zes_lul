using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using P06Shop.Api.Data;
using P06Shop.Api.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace P06Shop.Api.Tests
{
    public class TestBase : IDisposable
    {
        protected readonly HttpClient _client;
        protected readonly WebApplicationFactory<Program> _factory;
        protected readonly IMongoDatabase _database;
        protected readonly string _dbName = "TestDb";
        protected readonly JsonSerializerOptions _jsonOptions;
        protected const string BaseUrl = "http://localhost:8000/api";

        protected TestBase()
        {
            _factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        // Replace the real MongoDB with test database
                        services.AddSingleton<MongoDbContext>(sp =>
                            new MongoDbContext($"mongodb://localhost:27017", _dbName));
                    });
                });

            _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                BaseAddress = new Uri("http://localhost:8000")
            });

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var mongoClient = new MongoClient("mongodb://localhost:27017");
            _database = mongoClient.GetDatabase(_dbName);
        }

        protected async Task<string> GetAdminTokenAsync()
        {
            // Create admin user if not exists
            var users = _database.GetCollection<User>("Users");
            if (!await users.Find(x => x.Role == "Admin").AnyAsync())
            {
                var admin = new User
                {
                    Email = "admin@test.com",
                    Username = "admin",
                    FirstName = "Admin",
                    LastName = "User",
                    PasswordHash = HashPassword("admin123"),
                    Role = "Admin"
                };
                await users.InsertOneAsync(admin);
            }

            var loginResponse = await _client.PostAsync($"{BaseUrl}/Auth/login",
                new StringContent(JsonSerializer.Serialize(new LoginModel
                {
                    Email = "admin@test.com",
                    Password = "admin123"
                }), Encoding.UTF8, "application/json"));

            if (!loginResponse.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"Failed to login as admin: {await loginResponse.Content.ReadAsStringAsync()}");
            }

            var content = await loginResponse.Content.ReadAsStringAsync();
            var response = JsonSerializer.Deserialize<AuthResponse>(content, _jsonOptions);

            return response?.Token ?? throw new InvalidOperationException("Failed to get admin token");
        }

        protected async Task<string> CreateUserAndGetTokenAsync(string email = "user@test.com")
        {
            var registerModel = new RegisterModel
            {
                Email = email,
                Password = "user123",
                Username = "testuser",
                FirstName = "Test",
                LastName = "User"
            };

            var registerResponse = await _client.PostAsync($"{BaseUrl}/Auth/register",
                new StringContent(JsonSerializer.Serialize(registerModel), Encoding.UTF8, "application/json"));

            if (!registerResponse.IsSuccessStatusCode)
            {
                // Try logging in if registration fails (user might already exist)
                var loginResponse = await _client.PostAsync($"{BaseUrl}/Auth/login",
                    new StringContent(JsonSerializer.Serialize(new LoginModel
                    {
                        Email = email,
                        Password = "user123"
                    }), Encoding.UTF8, "application/json"));

                if (!loginResponse.IsSuccessStatusCode)
                {
                    throw new InvalidOperationException($"Failed to register/login user: {await loginResponse.Content.ReadAsStringAsync()}");
                }

                var loginContent = await loginResponse.Content.ReadAsStringAsync();
                var loginResult = JsonSerializer.Deserialize<AuthResponse>(loginContent, _jsonOptions);
                return loginResult?.Token ?? throw new InvalidOperationException("Failed to get user token");
            }

            var content = await registerResponse.Content.ReadAsStringAsync();
            var response = JsonSerializer.Deserialize<AuthResponse>(content, _jsonOptions);

            return response?.Token ?? throw new InvalidOperationException("Failed to get user token");
        }

        protected HttpClient CreateAuthenticatedClient(string token)
        {
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                BaseAddress = new Uri("http://localhost:8000")
            });
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return client;
        }

        private static string HashPassword(string password)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }

        public void Dispose()
        {
            var client = new MongoClient("mongodb://localhost:27017");
            client.DropDatabase(_dbName);
            _factory.Dispose();
        }
    }
} 
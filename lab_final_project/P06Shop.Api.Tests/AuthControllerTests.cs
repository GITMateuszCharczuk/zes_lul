using System.Net;
using System.Text;
using System.Text.Json;
using P06Shop.Api.Models;
using Xunit;

namespace P06Shop.Api.Tests
{
    public class AuthControllerTests : TestBase
    {
        [Fact]
        public async Task Register_WithValidData_ReturnsSuccess()
        {
            // Arrange
            var registerModel = new RegisterModel
            {
                Email = "test@example.com",
                Password = "password123",
                Username = "testuser",
                FirstName = "Test",
                LastName = "User"
            };

            // Act
            var response = await _client.PostAsync($"{BaseUrl}/Auth/register",
                new StringContent(JsonSerializer.Serialize(registerModel, _jsonOptions), Encoding.UTF8, "application/json"));
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<AuthResponse>(content, _jsonOptions);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(result?.Token);
            Assert.Equal(registerModel.Username, result.Username);
            Assert.Equal("Customer", result.Role);
        }

        [Fact]
        public async Task Register_WithDuplicateEmail_ReturnsBadRequest()
        {
            // Arrange
            var email = "duplicate@example.com";
            await CreateUserAndGetTokenAsync(email);

            var registerModel = new RegisterModel
            {
                Email = email,
                Password = "password123",
                Username = "testuser2",
                FirstName = "Test",
                LastName = "User"
            };

            // Act
            var response = await _client.PostAsync($"{BaseUrl}/Auth/register",
                new StringContent(JsonSerializer.Serialize(registerModel, _jsonOptions), Encoding.UTF8, "application/json"));

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Login_WithValidCredentials_ReturnsToken()
        {
            // Arrange
            var email = "login@example.com";
            await CreateUserAndGetTokenAsync(email);

            var loginModel = new LoginModel
            {
                Email = email,
                Password = "user123"
            };

            // Act
            var response = await _client.PostAsync($"{BaseUrl}/Auth/login",
                new StringContent(JsonSerializer.Serialize(loginModel, _jsonOptions), Encoding.UTF8, "application/json"));
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<AuthResponse>(content, _jsonOptions);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(result?.Token);
        }

        [Fact]
        public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
        {
            // Arrange
            var loginModel = new LoginModel
            {
                Email = "nonexistent@example.com",
                Password = "wrongpassword"
            };

            // Act
            var response = await _client.PostAsync($"{BaseUrl}/Auth/login",
                new StringContent(JsonSerializer.Serialize(loginModel, _jsonOptions), Encoding.UTF8, "application/json"));

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task PromoteToAdmin_AsAdmin_ReturnsSuccess()
        {
            // Arrange
            var adminToken = await GetAdminTokenAsync();
            var userToken = await CreateUserAndGetTokenAsync("promote@example.com");
            var client = CreateAuthenticatedClient(adminToken);

            // Get user ID from token
            var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(userToken) as System.IdentityModel.Tokens.Jwt.JwtSecurityToken;
            var userId = jsonToken?.Claims.First(claim => claim.Type == "nameid").Value;

            // Act
            var response = await client.PostAsync($"{BaseUrl}/Auth/promote/{userId}", null);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task PromoteToAdmin_AsNonAdmin_ReturnsForbidden()
        {
            // Arrange
            var userToken = await CreateUserAndGetTokenAsync();
            var client = CreateAuthenticatedClient(userToken);

            // Act
            var response = await client.PostAsync($"{BaseUrl}/Auth/promote/someUserId", null);

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }
    }
} 
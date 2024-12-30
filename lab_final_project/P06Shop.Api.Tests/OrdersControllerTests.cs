using System.Net;
using System.Text;
using System.Text.Json;
using MongoDB.Driver;
using P06Shop.Api.Models;
using Xunit;
using System.IdentityModel.Tokens.Jwt;

namespace P06Shop.Api.Tests
{
    public class OrdersControllerTests : TestBase
    {
        private async Task<Product> CreateTestProduct()
        {
            var product = new Product
            {
                Title = "Test Product",
                Description = "Test Description",
                Barcode = "123456789",
                Price = 9.99,
                ReleaseDate = DateTime.UtcNow
            };

            await _database.GetCollection<Product>("Products").InsertOneAsync(product);
            return product;
        }

        private string GetUserIdFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
            return jsonToken?.Claims.First(claim => claim.Type == "nameid").Value 
                ?? throw new InvalidOperationException("User ID not found in token");
        }

        [Fact]
        public async Task GetOrders_AsAdmin_ReturnsAllOrders()
        {
            // Arrange
            var adminToken = await GetAdminTokenAsync();
            var client = CreateAuthenticatedClient(adminToken);

            // Act
            var response = await client.GetAsync($"{BaseUrl}/Orders");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetOrders_AsCustomer_ReturnsForbidden()
        {
            // Arrange
            var userToken = await CreateUserAndGetTokenAsync();
            var client = CreateAuthenticatedClient(userToken);

            // Act
            var response = await client.GetAsync($"{BaseUrl}/Orders");

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task CreateOrder_WithValidData_ReturnsCreated()
        {
            // Arrange
            var userToken = await CreateUserAndGetTokenAsync();
            var userId = GetUserIdFromToken(userToken);
            var client = CreateAuthenticatedClient(userToken);
            var product = await CreateTestProduct();

            var order = new Order
            {
                UserId = userId,
                Items = new List<OrderItem>
                {
                    new OrderItem
                    {
                        ProductId = product.Id,
                        Quantity = 2
                    }
                },
                Status = "Pending"
            };

            // Act
            var response = await client.PostAsync($"{BaseUrl}/Orders",
                new StringContent(JsonSerializer.Serialize(order, _jsonOptions), Encoding.UTF8, "application/json"));

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<Order>(content, _jsonOptions);
            Assert.NotNull(result);
            Assert.Equal(2 * product.Price, (double)result.TotalAmount);
        }

        [Fact]
        public async Task GetOrder_AsOwner_ReturnsOrder()
        {
            // Arrange
            var userToken = await CreateUserAndGetTokenAsync();
            var userId = GetUserIdFromToken(userToken);
            var client = CreateAuthenticatedClient(userToken);
            var product = await CreateTestProduct();

            // Create an order first
            var order = new Order
            {
                UserId = userId,
                Items = new List<OrderItem>
                {
                    new OrderItem
                    {
                        ProductId = product.Id,
                        Quantity = 1
                    }
                },
                Status = "Pending"
            };

            var createResponse = await client.PostAsync($"{BaseUrl}/Orders",
                new StringContent(JsonSerializer.Serialize(order, _jsonOptions), Encoding.UTF8, "application/json"));
            var content = await createResponse.Content.ReadAsStringAsync();
            var createdOrder = JsonSerializer.Deserialize<Order>(content, _jsonOptions);

            // Act
            var response = await client.GetAsync($"{BaseUrl}/Orders/{createdOrder?.Id}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task UpdateOrderStatus_AsAdmin_ReturnsSuccess()
        {
            // Arrange
            var userToken = await CreateUserAndGetTokenAsync();
            var userId = GetUserIdFromToken(userToken);
            var adminToken = await GetAdminTokenAsync();
            var userClient = CreateAuthenticatedClient(userToken);
            var adminClient = CreateAuthenticatedClient(adminToken);
            var product = await CreateTestProduct();

            // Create an order first
            var order = new Order
            {
                UserId = userId,
                Items = new List<OrderItem>
                {
                    new OrderItem
                    {
                        ProductId = product.Id,
                        Quantity = 1
                    }
                },
                Status = "Pending"
            };

            var createResponse = await userClient.PostAsync($"{BaseUrl}/Orders",
                new StringContent(JsonSerializer.Serialize(order, _jsonOptions), Encoding.UTF8, "application/json"));
            var content = await createResponse.Content.ReadAsStringAsync();
            var createdOrder = JsonSerializer.Deserialize<Order>(content, _jsonOptions);

            // Act
            var response = await adminClient.PutAsync($"{BaseUrl}/Orders/{createdOrder?.Id}/status",
                new StringContent("\"Shipped\"", Encoding.UTF8, "application/json"));

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task DeleteOrder_AsOwner_ReturnsSuccess()
        {
            // Arrange
            var userToken = await CreateUserAndGetTokenAsync();
            var userId = GetUserIdFromToken(userToken);
            var client = CreateAuthenticatedClient(userToken);
            var product = await CreateTestProduct();

            // Create an order first
            var order = new Order
            {
                UserId = userId,
                Items = new List<OrderItem>
                {
                    new OrderItem
                    {
                        ProductId = product.Id,
                        Quantity = 1
                    }
                },
                Status = "Pending"
            };

            var createResponse = await client.PostAsync($"{BaseUrl}/Orders",
                new StringContent(JsonSerializer.Serialize(order, _jsonOptions), Encoding.UTF8, "application/json"));
            var content = await createResponse.Content.ReadAsStringAsync();
            var createdOrder = JsonSerializer.Deserialize<Order>(content, _jsonOptions);

            // Act
            var response = await client.DeleteAsync($"{BaseUrl}/Orders/{createdOrder?.Id}");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task DeleteOrder_AsNonOwner_ReturnsForbidden()
        {
            // Arrange
            var user1Token = await CreateUserAndGetTokenAsync("user1@test.com");
            var user1Id = GetUserIdFromToken(user1Token);
            var user2Token = await CreateUserAndGetTokenAsync("user2@test.com");
            var user1Client = CreateAuthenticatedClient(user1Token);
            var user2Client = CreateAuthenticatedClient(user2Token);
            var product = await CreateTestProduct();

            // Create an order as user1
            var order = new Order
            {
                UserId = user1Id,
                Items = new List<OrderItem>
                {
                    new OrderItem
                    {
                        ProductId = product.Id,
                        Quantity = 1
                    }
                },
                Status = "Pending"
            };

            var createResponse = await user1Client.PostAsync($"{BaseUrl}/Orders",
                new StringContent(JsonSerializer.Serialize(order, _jsonOptions), Encoding.UTF8, "application/json"));
            var content = await createResponse.Content.ReadAsStringAsync();
            var createdOrder = JsonSerializer.Deserialize<Order>(content, _jsonOptions);

            // Act - try to delete as user2
            var response = await user2Client.DeleteAsync($"{BaseUrl}/Orders/{createdOrder?.Id}");

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }
    }
} 
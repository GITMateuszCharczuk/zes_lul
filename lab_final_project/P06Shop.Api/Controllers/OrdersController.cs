using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MongoDB.Driver;
using P06Shop.Api.Data;
using P06Shop.Api.Models;
using System.Security.Claims;

namespace P06Shop.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly MongoDbContext _context;

        public OrdersController(MongoDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            var orders = await _context.Orders.Find(_ => true).ToListAsync();
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(string id)
        {
            var order = await _context.Orders.Find(x => x.Id == id).FirstOrDefaultAsync();
            if (order == null)
                return NotFound();

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (order.UserId != userId && !User.IsInRole("Admin"))
                return Forbid();

            return Ok(order);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<Order>>> GetUserOrders(string userId)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId != currentUserId && !User.IsInRole("Admin"))
                return Forbid();

            var orders = await _context.Orders.Find(x => x.UserId == userId).ToListAsync();
            return Ok(orders);
        }

        [HttpPost]
        public async Task<ActionResult<Order>> CreateOrder(Order order)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            order.UserId = userId!;

            // Calculate total amount based on products
            decimal total = 0;
            foreach (var item in order.Items)
            {
                var product = await _context.Products.Find(x => x.Id == item.ProductId).FirstOrDefaultAsync();
                if (product == null)
                    return BadRequest($"Product with ID {item.ProductId} not found");
                
                item.PriceAtOrder = (decimal)product.Price;
                total += item.PriceAtOrder * item.Quantity;
            }
            
            order.TotalAmount = total;
            await _context.Orders.InsertOneAsync(order);

            // Update user's order list
            var update = Builders<User>.Update.Push(u => u.OrderIds, order.Id);
            await _context.Users.UpdateOneAsync(u => u.Id == order.UserId, update);

            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(string id, Order order)
        {
            var existingOrder = await _context.Orders.Find(x => x.Id == id).FirstOrDefaultAsync();
            if (existingOrder == null)
                return NotFound();

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (existingOrder.UserId != userId && !User.IsInRole("Admin"))
                return Forbid();

            var result = await _context.Orders.ReplaceOneAsync(x => x.Id == id, order);
            if (result.ModifiedCount == 0)
                return NotFound();

            return NoContent();
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateOrderStatus(string id, [FromBody] string status)
        {
            var update = Builders<Order>.Update.Set(o => o.Status, status);
            var result = await _context.Orders.UpdateOneAsync(x => x.Id == id, update);
            
            if (result.ModifiedCount == 0)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(string id)
        {
            var order = await _context.Orders.Find(x => x.Id == id).FirstOrDefaultAsync();
            if (order == null)
                return NotFound();

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (order.UserId != userId && !User.IsInRole("Admin"))
                return Forbid();

            // Remove order ID from user's order list
            var update = Builders<User>.Update.Pull(u => u.OrderIds, id);
            await _context.Users.UpdateOneAsync(u => u.Id == order.UserId, update);

            // Delete the order
            await _context.Orders.DeleteOneAsync(x => x.Id == id);

            return NoContent();
        }
    }
} 
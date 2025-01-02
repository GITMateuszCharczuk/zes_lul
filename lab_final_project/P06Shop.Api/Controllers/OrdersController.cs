using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MongoDB.Driver;
using P06Shop.Api.Data;
using P06Shop.Api.Models;
using P06Shop.Api.Models.Dto;
using P06Shop.Api.Mappers.Interfaces;
using System.Security.Claims;

namespace P06Shop.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly MongoDbContext _context;
        private readonly IOrderMapper _orderMapper;

        public OrdersController(MongoDbContext context, IOrderMapper orderMapper)
        {
            _context = context;
            _orderMapper = orderMapper;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<GetOrderDTO>>> GetOrders()
        {
            var orders = await _context.Orders.Find(_ => true).ToListAsync();
            var orderDtos = _orderMapper.MapToGetOrderDTOs(orders);
            return Ok(orderDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GetOrderDTO>> GetOrder(string id)
        {
            var order = await _context.Orders.Find(x => x.Id == id).FirstOrDefaultAsync();
            if (order == null)
                return NotFound();

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (order.UserId != userId && !User.IsInRole("Admin"))
                return Forbid();

            var orderDto = _orderMapper.MapToGetOrderDTO(order);
            return Ok(orderDto);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<GetOrderDTO>>> GetUserOrders(string userId)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId != currentUserId && !User.IsInRole("Admin"))
                return Forbid();

            var orders = await _context.Orders.Find(x => x.UserId == userId).ToListAsync();
            var orderDtos = _orderMapper.MapToGetOrderDTOs(orders);
            return Ok(orderDtos);
        }

        [HttpPost]
        public async Task<ActionResult<GetOrderDTO>> CreateOrder(CreateOrderDTO orderDto)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (currentUserId == null)
                return Unauthorized();

            string userId;
            if (!string.IsNullOrEmpty(orderDto.UserId))
            {
                userId = orderDto.UserId;
            }
            else
            {
                userId = currentUserId;
            }

            // Verify that the user exists
            var user = await _context.Users.Find(x => x.Id == userId).FirstOrDefaultAsync();
            if (user == null)
                return BadRequest("Invalid user ID");

            // Calculate total amount based on products
            decimal total = 0;
            var orderItems = new List<OrderItem>();
            foreach (var item in orderDto.Items)
            {
                var product = await _context.Products.Find(x => x.Id == item.ProductId).FirstOrDefaultAsync();
                if (product == null)
                    return BadRequest($"Product with ID {item.ProductId} not found");
                
                var orderItem = new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    PriceAtOrder = (decimal)product.Price
                };
                orderItems.Add(orderItem);
                total += orderItem.PriceAtOrder * orderItem.Quantity;
            }
            
            var order = _orderMapper.MapCreateDTOToOrder(orderDto, userId, orderItems, total);
            await _context.Orders.InsertOneAsync(order);

            // Update user's order list
            var update = Builders<User>.Update.Push(u => u.OrderIds, order.Id);
            await _context.Users.UpdateOneAsync(u => u.Id == order.UserId, update);

            var createdOrderDto = _orderMapper.MapToGetOrderDTO(order);
            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, createdOrderDto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<GetOrderDTO>> UpdateOrder(string id, UpdateOrderDTO updateDto)
        {
            var existingOrder = await _context.Orders.Find(x => x.Id == id).FirstOrDefaultAsync();
            if (existingOrder == null)
                return NotFound();

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();

            if (existingOrder.UserId != userId && !User.IsInRole("Admin"))
                return Forbid();

            // Calculate total amount based on products
            decimal total = 0;
            var orderItems = new List<OrderItem>();
            foreach (var item in updateDto.Items)
            {
                var product = await _context.Products.Find(x => x.Id == item.ProductId).FirstOrDefaultAsync();
                if (product == null)
                    return BadRequest($"Product with ID {item.ProductId} not found");

                var orderItem = new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    PriceAtOrder = (decimal)product.Price
                };
                orderItems.Add(orderItem);
                total += orderItem.PriceAtOrder * orderItem.Quantity;
            }

            var updatedOrder = _orderMapper.MapUpdateDTOToOrder(id, updateDto, existingOrder, orderItems, total);
            var result = await _context.Orders.ReplaceOneAsync(x => x.Id == id, updatedOrder);
            if (result.ModifiedCount == 0)
                return NotFound();

            var updatedOrderDto = _orderMapper.MapToGetOrderDTO(updatedOrder);
            return Ok(updatedOrderDto);
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteOrder(string id)
        {
            var order = await _context.Orders.Find(x => x.Id == id).FirstOrDefaultAsync();
            if (order == null)
                return NotFound();

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();

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
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using P06Shop.Api.Models.Dto;
using P06Shop.Api.Services;
using System.Security.Claims;

namespace P06Shop.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly OrderService _orderService;

        public OrdersController(OrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<GetOrderDTO>>> GetOrders()
        {
            var result = await _orderService.GetOrders();
            if (!result.Success)
                return StatusCode(500, result.ErrorMessage);

            return Ok(result.Orders);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GetOrderDTO>> GetOrder(string id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();

            var result = await _orderService.GetOrder(id, userId, User.IsInRole("Admin"));
            if (!result.Success)
            {
                if (result.ErrorMessage == "Forbidden")
                    return Forbid();
                return NotFound(result.ErrorMessage);
            }

            return Ok(result.Order);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<GetOrderDTO>>> GetUserOrders(string userId)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (currentUserId == null)
                return Unauthorized();

            var result = await _orderService.GetUserOrders(userId, currentUserId, User.IsInRole("Admin"));
            if (!result.Success)
            {
                if (result.ErrorMessage == "Forbidden")
                    return Forbid();
                return StatusCode(500, result.ErrorMessage);
            }

            return Ok(result.Orders);
        }

        [HttpPost]
        public async Task<ActionResult<GetOrderDTO>> CreateOrder(CreateOrderDTO orderDto)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (currentUserId == null)
                return Unauthorized();

            var result = await _orderService.CreateOrder(orderDto, currentUserId);
            if (!result.Success)
                return BadRequest(result.ErrorMessage);

            return CreatedAtAction(nameof(GetOrder), new { id = result.Order!.Id }, result.Order);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<GetOrderDTO>> UpdateOrder(string id, UpdateOrderDTO updateDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();

            var result = await _orderService.UpdateOrder(id, updateDto, userId, User.IsInRole("Admin"));
            if (!result.Success)
            {
                if (result.ErrorMessage == "Forbidden")
                    return Forbid();
                return NotFound(result.ErrorMessage);
            }

            return Ok(result.Order);
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateOrderStatus(string id, [FromBody] string status)
        {
            var result = await _orderService.UpdateOrderStatus(id, status);
            if (!result.Success)
                return NotFound(result.ErrorMessage);

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteOrder(string id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();

            var result = await _orderService.DeleteOrder(id, userId, User.IsInRole("Admin"));
            if (!result.Success)
            {
                if (result.ErrorMessage == "Forbidden")
                    return Forbid();
                return NotFound(result.ErrorMessage);
            }

            return NoContent();
        }
    }
} 
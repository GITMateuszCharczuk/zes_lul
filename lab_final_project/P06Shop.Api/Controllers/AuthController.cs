using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using P06Shop.Api.Models;
using P06Shop.Api.Services;

namespace P06Shop.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest model)
        {
            var response = await _authService.Login(model.Email, model.Password);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest model)
        {
            var response = await _authService.Register(
                model.Username,
                model.Email,
                model.Password,
                model.FirstName,
                model.LastName
            );

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPost("promote/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PromoteToAdmin(string userId)
        {
            var result = await _authService.PromoteToAdmin(userId);
            if (!result.Success)
                return NotFound("User not found");

            return Ok("User promoted to Admin");
        }

        [HttpPost("demote/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DemoteToCustomer(string userId)
        {
            var result = await _authService.DemoteToCustomer(userId);
            if (!result.Success)
                return BadRequest("Cannot demote the last admin or user not found");

            return Ok("User demoted to Customer");
        }
    }
} 
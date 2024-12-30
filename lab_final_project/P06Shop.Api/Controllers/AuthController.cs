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
        public async Task<ActionResult<AuthResponse>> Login(LoginModel model)
        {
            var response = await _authService.Login(model);
            if (response == null)
                return Unauthorized("Invalid email or password");

            return Ok(response);
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponse>> Register(RegisterModel model)
        {
            var response = await _authService.Register(model);
            if (response == null)
                return BadRequest("Email or username already exists");

            return Ok(response);
        }

        [HttpPost("promote/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PromoteToAdmin(string userId)
        {
            var result = await _authService.PromoteToAdmin(userId);
            if (!result)
                return NotFound("User not found");

            return Ok("User promoted to Admin");
        }

        [HttpPost("demote/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DemoteToCustomer(string userId)
        {
            var result = await _authService.DemoteToCustomer(userId);
            if (!result)
                return BadRequest("Cannot demote the last admin or user not found");

            return Ok("User demoted to Customer");
        }
    }
} 
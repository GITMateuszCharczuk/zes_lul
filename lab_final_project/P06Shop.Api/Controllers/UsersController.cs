using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using P06Shop.Api.Models.Dto;
using P06Shop.Api.Services;
using System.Security.Claims;

namespace P06Shop.Api.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;

        public UsersController(UserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult<GetUsersDTO>> GetUsers()
        {
            var result = await _userService.GetUsers();
            if (!result.Success)
                return StatusCode(500, result.ErrorMessage);

            return Ok(result.Users);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GetUserDTO>> GetUser(string id)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (currentUserId == null)
                return Unauthorized();

            var result = await _userService.GetUser(id, currentUserId, User.IsInRole("Admin"));
            if (!result.Success)
            {
                if (result.ErrorMessage == "Forbidden")
                    return Forbid();
                return NotFound(result.ErrorMessage);
            }

            return Ok(result.User);
        }

        [HttpPost]
        public async Task<ActionResult<GetUserDTO>> CreateUser(CreateUserDTO createDto)
        {
            var result = await _userService.CreateUser(createDto);
            if (!result.Success)
                return BadRequest(result.ErrorMessage);

            return CreatedAtAction(nameof(GetUser), new { id = result.User!.Id }, result.User);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<GetUserDTO>> UpdateUser(string id, UpdateUserDTO updateDto)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (currentUserId == null)
                return Unauthorized();

            var result = await _userService.UpdateUser(id, updateDto, currentUserId, User.IsInRole("Admin"));
            if (!result.Success)
            {
                if (result.ErrorMessage == "Forbidden")
                    return Forbid();
                return NotFound(result.ErrorMessage);
            }

            return Ok(result.User);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var result = await _userService.DeleteUser(id);
            if (!result.Success)
                return NotFound(result.ErrorMessage);

            return NoContent();
        }
    }
} 
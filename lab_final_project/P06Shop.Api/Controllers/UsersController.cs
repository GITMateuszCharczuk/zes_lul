using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MongoDB.Driver;
using P06Shop.Api.Data;
using P06Shop.Api.Models;
using P06Shop.Api.Models.Dto;
using P06Shop.Api.Services;
using P06Shop.Api.Mappers.Interfaces;
using System.Security.Claims;

namespace P06Shop.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly MongoDbContext _context;
        private readonly AuthService _authService;
        private readonly IUserMapper _userMapper;

        public UsersController(MongoDbContext context, AuthService authService, IUserMapper userMapper)
        {
            _context = context;
            _authService = authService;
            _userMapper = userMapper;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<GetUsersDTO>> GetUsers()
        {
            try
            {
                var users = await _context.Users.Find(_ => true).ToListAsync();
                var userDtos = _userMapper.MapToGetUserDTOs(users);
                return Ok(new GetUsersDTO { Users = userDtos.ToList() });
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error while retrieving users.");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GetUserDTO>> GetUser(string id)
        {
            try
            {
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (id != currentUserId && !User.IsInRole("Admin"))
                    return Forbid();

                var user = await _context.Users.Find(x => x.Id == id).FirstOrDefaultAsync();
                if (user == null)
                    return NotFound("User not found.");

                var userDto = _userMapper.MapToGetUserDTO(user);
                return Ok(userDto);
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error while retrieving user.");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<GetUserDTO>> CreateUser(CreateUserDTO createDto)
        {
            try
            {
                // Check if username or email already exists
                var existingUser = await _context.Users.Find(u => 
                    u.Username == createDto.Username || 
                    u.Email == createDto.Email
                ).FirstOrDefaultAsync();

                if (existingUser != null)
                    return BadRequest("Username or email already exists.");

                var passwordHash = _authService.HashPassword(createDto.Password);
                var user = _userMapper.MapCreateDTOToUser(createDto, passwordHash);

                await _context.Users.InsertOneAsync(user);

                var userDto = _userMapper.MapToGetUserDTO(user);
                return CreatedAtAction(nameof(GetUser), new { id = user.Id }, userDto);
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error while creating user.");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<GetUserDTO>> UpdateUser(string id, UpdateUserDTO updateDto)
        {
            try
            {
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (id != currentUserId && !User.IsInRole("Admin"))
                    return Forbid();

                var existingUser = await _context.Users.Find(x => x.Id == id).FirstOrDefaultAsync();
                if (existingUser == null)
                    return NotFound("User not found.");

                // Check if new username already exists
                if (updateDto.Username != null)
                {
                    var duplicateUser = await _context.Users.Find(u =>
                        u.Id != id && u.Username == updateDto.Username
                    ).FirstOrDefaultAsync();

                    if (duplicateUser != null)
                        return BadRequest("Username already exists.");
                }

                var user = _userMapper.MapUpdateDTOToUser(id, updateDto, existingUser);
                var result = await _context.Users.ReplaceOneAsync(x => x.Id == id, user);
                if (result.ModifiedCount == 0)
                    return NotFound("User not found.");

                var userDto = _userMapper.MapToGetUserDTO(user);
                return Ok(userDto);
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error while updating user.");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            try
            {
                var result = await _context.Users.DeleteOneAsync(x => x.Id == id);
                if (result.DeletedCount == 0)
                    return NotFound("User not found.");

                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error while deleting user.");
            }
        }
    }
} 
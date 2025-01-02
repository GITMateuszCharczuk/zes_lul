using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using P06Shop.Api.Data;
using P06Shop.Api.Models;
using P06Shop.Api.Models.Dto;
using P06Shop.Api.Mappers.Interfaces;

namespace P06Shop.Api.Services
{
    public class UserService
    {
        private readonly MongoDbContext _context;
        private readonly AuthService _authService;
        private readonly IUserMapper _userMapper;

        public UserService(MongoDbContext context, AuthService authService, IUserMapper userMapper)
        {
            _context = context;
            _authService = authService;
            _userMapper = userMapper;
        }

        public async Task<(bool Success, GetUsersDTO? Users, string? ErrorMessage)> GetUsers()
        {
            try
            {
                var users = await _context.Users.Find(_ => true).ToListAsync();
                var userDtos = _userMapper.MapToGetUserDTOs(users);
                return (true, new GetUsersDTO { Users = userDtos.ToList() }, null);
            }
            catch (Exception ex)
            {
                return (false, null, $"Internal server error while retrieving users: {ex.Message}");
            }
        }

        public async Task<(bool Success, GetUserDTO? User, string? ErrorMessage)> GetUser(string id, string currentUserId, bool isAdmin)
        {
            try
            {
                if (id != currentUserId && !isAdmin)
                    return (false, null, "Forbidden");

                var user = await _context.Users.Find(x => x.Id == id).FirstOrDefaultAsync();
                if (user == null)
                    return (false, null, "User not found.");

                var userDto = _userMapper.MapToGetUserDTO(user);
                return (true, userDto, null);
            }
            catch (Exception ex)
            {
                return (false, null, $"Internal server error while retrieving user: {ex.Message}");
            }
        }

        public async Task<(bool Success, GetUserDTO? User, string? ErrorMessage)> CreateUser(CreateUserDTO createDto)
        {
            try
            {
                // Check if username or email already exists
                var existingUser = await _context.Users.Find(u => 
                    u.Username == createDto.Username || 
                    u.Email == createDto.Email
                ).FirstOrDefaultAsync();

                if (existingUser != null)
                    return (false, null, "Username or email already exists.");

                var passwordHash = _authService.HashPassword(createDto.Password);
                var user = _userMapper.MapCreateDTOToUser(createDto, passwordHash);

                await _context.Users.InsertOneAsync(user);

                var userDto = _userMapper.MapToGetUserDTO(user);
                return (true, userDto, null);
            }
            catch (Exception ex)
            {
                return (false, null, $"Internal server error while creating user: {ex.Message}");
            }
        }

        public async Task<(bool Success, GetUserDTO? User, string? ErrorMessage)> UpdateUser(string id, UpdateUserDTO updateDto, string currentUserId, bool isAdmin)
        {
            try
            {
                if (id != currentUserId && !isAdmin)
                    return (false, null, "Forbidden");

                var existingUser = await _context.Users.Find(x => x.Id == id).FirstOrDefaultAsync();
                if (existingUser == null)
                    return (false, null, "User not found.");

                // Check if new username already exists
                if (updateDto.Username != null)
                {
                    var duplicateUser = await _context.Users.Find(u =>
                        u.Id != id && u.Username == updateDto.Username
                    ).FirstOrDefaultAsync();

                    if (duplicateUser != null)
                        return (false, null, "Username already exists.");
                }

                var user = _userMapper.MapUpdateDTOToUser(id, updateDto, existingUser);
                var result = await _context.Users.ReplaceOneAsync(x => x.Id == id, user);
                if (result.ModifiedCount == 0)
                    return (false, null, "User not found.");

                var userDto = _userMapper.MapToGetUserDTO(user);
                return (true, userDto, null);
            }
            catch (Exception ex)
            {
                return (false, null, $"Internal server error while updating user: {ex.Message}");
            }
        }

        public async Task<(bool Success, string? ErrorMessage)> DeleteUser(string id)
        {
            try
            {
                var result = await _context.Users.DeleteOneAsync(x => x.Id == id);
                if (result.DeletedCount == 0)
                    return (false, "User not found.");

                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, $"Internal server error while deleting user: {ex.Message}");
            }
        }
    }
} 
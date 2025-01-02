using P06Shop.Api.Mappers.Interfaces;
using P06Shop.Api.Models;
using P06Shop.Api.Models.Dto;
using MongoDB.Bson;

namespace P06Shop.Api.Mappers
{
    public class UserMapper : IUserMapper
    {
        public GetUserDTO MapToGetUserDTO(User user)
        {
            return new GetUserDTO
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role,
                OrderIds = user.OrderIds
            };
        }

        public IEnumerable<GetUserDTO> MapToGetUserDTOs(IEnumerable<User> users)
        {
            return users.Select(MapToGetUserDTO);
        }

        public User MapCreateDTOToUser(CreateUserDTO createDTO, string passwordHash)
        {
            return new User
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Username = createDTO.Username,
                Email = createDTO.Email,
                FirstName = createDTO.FirstName,
                LastName = createDTO.LastName,
                PasswordHash = passwordHash,
                Role = "Customer",
                CreatedAt = DateTime.UtcNow,
                OrderIds = new List<string>()
            };
        }

        public User MapUpdateDTOToUser(string id, UpdateUserDTO updateDTO, User existingUser)
        {
            return new User
            {
                Id = id,
                Username = updateDTO.Username ?? existingUser.Username,
                Email = existingUser.Email,
                FirstName = updateDTO.FirstName ?? existingUser.FirstName,
                LastName = updateDTO.LastName ?? existingUser.LastName,
                PasswordHash = existingUser.PasswordHash,
                Role = existingUser.Role,
                CreatedAt = existingUser.CreatedAt,
                OrderIds = existingUser.OrderIds
            };
        }
    }
} 
using P06Shop.Api.Models;
using P06Shop.Api.Models.Dto;

namespace P06Shop.Api.Mappers.Interfaces
{
    public interface IUserMapper
    {
        GetUserDTO MapToGetUserDTO(User user);
        IEnumerable<GetUserDTO> MapToGetUserDTOs(IEnumerable<User> users);
        User MapCreateDTOToUser(CreateUserDTO createDTO, string passwordHash);
        User MapUpdateDTOToUser(string id, UpdateUserDTO updateDTO, User existingUser);
    }
} 
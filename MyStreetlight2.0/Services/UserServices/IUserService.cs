using Streetlight2._0.DTOs.UserDtos;

namespace Streetlight2._0.Services.UserService
{
    public interface IUserService
    {
        Task<bool> IsUserValid(string userName);

        Task<bool> IsPasswordValid(string userName, string password);

        Task<UserDto> GetUserByUserName(string userName);

        Task<List<UserPermissionDto>> GetUserPermissionsByUserId(int userId);

        Task<string[]> GetUserPermissionsArrayByUserId(int userId);

        Task<string> GetUserNameByUserIdAsync(int userId);
    }
}

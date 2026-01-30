using Microsoft.EntityFrameworkCore;
using Streetlight2._0.Data;
using Streetlight2._0.DTOs.UserDtos;

namespace Streetlight2._0.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<UserService> _logger;

        public UserService(
            AppDbContext dbContext, 
            ILogger<UserService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<bool> IsUserValid(string userName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userName))
                {
                    return false;
                }

                return await _dbContext.Users.AnyAsync(u => u.UserName == userName);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> IsPasswordValid(string userName, string password)
        {
            try
            {
                Console.WriteLine(userName + " " + password);
                if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
                {
                    return false;
                }

                return await _dbContext.Users.AnyAsync(u => u.UserName == userName && u.Password == password);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<UserDto> GetUserByUserName(string userName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userName))
                {
                    return null;
                }

                var user = await _dbContext.Users
                    .Where(u => u.UserName == userName)
                    .Select(u => new UserDto
                    {
                        Id = u.Id,
                        UserName = u.UserName
                    })
                    .FirstOrDefaultAsync();

                if (user == null)
                {
                    return null;
                }

                return user;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<List<UserPermissionDto>> GetUserPermissionsByUserId(int userId)
        {
            try
            {
                var permissions = await (from up in _dbContext.UserPermissions
                                         join p in _dbContext.Permissions on up.PermissionId equals p.Id
                                         where up.UserId == userId
                                         select new UserPermissionDto
                                         {
                                             PermissionId = p.Id,
                                             Name = p.Name
                                         }).ToListAsync();
                return permissions;
            }
            catch (Exception)
            {
                return new List<UserPermissionDto>();
            }
        }

        public async Task<string[]> GetUserPermissionsArrayByUserId(int userId)
        {
            try
            {
                var permissions = await GetUserPermissionsByUserId(userId);
                return permissions.Select(p => p.Name).ToArray();
            }
            catch (Exception)
            {
                return Array.Empty<string>();
            }
        }

        public async Task<string> GetUserNameByUserIdAsync(int userId)
        {
            try
            {
                var user = await _dbContext.Users
                    .FirstOrDefaultAsync(u => u.Id == userId);

                return user?.UserName;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching username for userId {UserId}", userId);

                throw;
            }
        }
    }
}

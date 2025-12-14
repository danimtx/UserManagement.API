using System.Threading.Tasks;
using UserManagement.Application.DTOs.User;
using UserManagement.Application.DTOs.User.Profile;
using UserManagement.Application.DTOs.Public;

namespace UserManagement.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task RequestTagAsync(string userId, RequestTagDto dto);
        Task RequestModuleAsync(string userId, RequestPersonalModuleDto dto);
        Task<UserProfileDto> GetUserProfileAsync(string userId);
        Task UpdateUserProfileAsync(string userId, UpdateProfileDto dto);
        Task<PublicUserProfileDto> GetPublicProfileAsync(string userId);
    }
}

using System.Threading.Tasks;
using UserManagement.Application.DTOs.User;

namespace UserManagement.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task RequestTagAsync(string userId, RequestTagDto dto);
    }
}

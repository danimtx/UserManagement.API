using System.Threading.Tasks;
using UserManagement.Application.DTOs.Review;

namespace UserManagement.Application.Interfaces.Services
{
    public interface IReviewService
    {
        Task CreateReviewAsync(string authorId, CreateReviewDto dto);
    }
}

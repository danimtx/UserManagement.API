using System.Threading.Tasks;
using UserManagement.Application.DTOs.Review;
using System.Collections.Generic; // Added

namespace UserManagement.Application.Interfaces.Services
{
    public interface IReviewService
    {
        Task CreateReviewAsync(string authorId, CreateReviewDto dto);
        Task<List<ReviewDetailDto>> GetReviewsAsync(string recipientId, string contextId);
    }
}

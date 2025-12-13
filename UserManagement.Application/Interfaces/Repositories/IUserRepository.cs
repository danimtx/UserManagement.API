using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UserManagement.Domain.Entities;

namespace UserManagement.Application.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(string id);
        Task<User?> GetByEmailAsync(string email);
        Task AddAsync(User user);
        Task UpdateAsync(User user);
        Task<User?> GetByUserNameAsync(string username);
        
        // --- Métodos para Admin ---
        Task<List<User>> GetPendingCompaniesAsync();
        Task<List<User>> GetCompaniesWithPendingModulesAsync();
        Task<List<User>> GetUsersWithPendingTagsAsync();

        // --- Métodos para Reviews ---
        Task AddReviewAsync(Review review);
        Task<bool> HasUserReviewedContextAsync(string authorId, string recipientId, string contextId);
    }
}

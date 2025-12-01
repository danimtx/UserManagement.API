using System;
using System.Collections.Generic;
using System.Text;
using UserManagement.Domain.Entities;

namespace UserManagement.Application.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(string id);
        Task<User?> GetByEmailAsync(string email);
        Task AddAsync(User user);
        Task UpdateAsync(User user);
        Task<List<User>> GetPendingCompaniesAsync();
        Task<List<User>> GetActivePersonalUsersAsync();
        Task<User?> GetByUserNameAsync(string username);
    }
}

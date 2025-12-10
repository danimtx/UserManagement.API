using System;
using System.Collections.Generic;
using System.Text;

namespace UserManagement.Application.Interfaces.Services
{
    public interface IIdentityProvider
    {
        Task<string> CreateUserAsync(string email, string password, string displayName);
        Task DeleteUserAsync(string uid);
        Task<(string Token, string Uid, string RefreshToken)> SignInAsync(string email, string password);
        Task<(string NewToken, string NewRefreshToken)> RefreshTokenAsync(string refreshToken);
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using UserManagement.Application.DTOs;

namespace UserManagement.Application.Interfaces
{
    public interface IAuthService
    {
        Task<string> RegisterAsync(RegisterUserDto userDto);
        Task<string> LoginAsync(LoginDto loginDto);
    }
}

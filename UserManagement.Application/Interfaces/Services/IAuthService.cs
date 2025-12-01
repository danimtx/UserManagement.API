using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UserManagement.Application.DTOs.Auth;

namespace UserManagement.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<string> RegisterPersonalAsync(RegisterPersonalDto dto);
        Task<string> RegisterCompanyAsync(RegisterCompanyDto dto);
        Task<string> LoginAsync(LoginDto loginDto);
    }
}
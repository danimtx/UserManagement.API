using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UserManagement.Application.DTOs.Admin;
using System.Text;

namespace UserManagement.Application.Interfaces.Services
{
    public interface IAdminService
    {
        Task<List<CompanyValidationDto>> GetPendingCompaniesAsync();
        Task<string> ApproveCompanyAsync(string userId);
        Task<List<MarketValidationDto>> GetPendingMarketUsersAsync();
        Task<string> VerifyMarketUserAsync(string userId);
    }
}
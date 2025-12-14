using System.Collections.Generic;
using System.Threading.Tasks;
using UserManagement.Application.DTOs.Company;
using UserManagement.Application.DTOs.Company.Employees;
using UserManagement.Application.DTOs.Public; // Added
using UserManagement.Domain.Enums;

namespace UserManagement.Application.Interfaces.Services
{
    public interface ICompanyService
    {
        Task<string> CreateEmployeeAsync(string companyId, CreateEmployeeDto dto);
        Task AddCompanyAreaAsync(string companyId, string newAreaName);

        Task RequestCommercialProfileAsync(string companyId, RequestCommercialProfileDto dto);
        Task RectifyIdentityAsync(string companyId, RectifyIdentityDto dto);

        // --- Employee Management ---
        Task<List<EmployeeSummaryDto>> GetEmployeesAsync(string companyId);
        Task<EmployeeDetailDto> GetEmployeeDetailAsync(string companyId, string employeeId);
        Task UpdateEmployeePermissionsAsync(string companyId, string employeeId, UpdateEmployeePermissionsDto dto);
        Task UpdateEmployeeStatusAsync(string companyId, string employeeId, UserStatus nuevoEstado);
        Task ResetEmployeePasswordAsync(string companyId, string employeeId, string newPassword);
        Task UpdateEmployeeProfileAsync(string companyId, string employeeId, UpdateEmployeeProfileDto dto);

        // --- Public Profiles ---
        Task<List<PublicCommercialProfileDto>> GetCompanyPublicProfilesAsync(string companyId);
    }
}
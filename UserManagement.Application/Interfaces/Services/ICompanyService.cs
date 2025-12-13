using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UserManagement.Application.DTOs.Company;

namespace UserManagement.Application.Interfaces.Services
{
    public interface ICompanyService
    {
        Task<string> CreateEmployeeAsync(string companyId, CreateEmployeeDto dto);
        Task AddCompanyAreaAsync(string companyId, string newAreaName);

        Task RequestCommercialProfileAsync(string companyId, RequestCommercialProfileDto dto);
        Task RectifyIdentityAsync(string companyId, RectifyIdentityDto dto);

        // Obtener lista de empleados (para la tabla de gestión)
        // Task<List<EmployeeSummaryDto>> GetEmployeesAsync(string companyId);
    }
}

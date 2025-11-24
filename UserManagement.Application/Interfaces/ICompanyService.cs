using System;
using System.Collections.Generic;
using System.Text;
using UserManagement.Application.DTOs.Company;

namespace UserManagement.Application.Interfaces
{
    public interface ICompanyService
    {
        // Crea un empleado vinculado a la empresa que hace la petición
        Task<string> CreateEmployeeAsync(string companyId, CreateEmployeeDto dto);

        // Agrega una nueva área (Ej: "Logística") al perfil de la empresa
        Task AddCompanyAreaAsync(string companyId, string newAreaName);

        // Obtener lista de empleados (para la tabla de gestión)
        // Task<List<EmployeeSummaryDto>> GetEmployeesAsync(string companyId);
    }
}

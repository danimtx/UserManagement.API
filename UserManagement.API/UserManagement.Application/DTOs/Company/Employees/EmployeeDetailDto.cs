using System.Collections.Generic;
using UserManagement.Application.DTOs.Company;

namespace UserManagement.Application.DTOs.Company.Employees
{
    public class EmployeeDetailDto : EmployeeSummaryDto
    {
        public string? CI { get; set; }
        public string? Celular { get; set; }
        public string? Direccion { get; set; }
        public Dictionary<string, ModuleAccessDto> Permisos { get; set; } = new();
    }
}
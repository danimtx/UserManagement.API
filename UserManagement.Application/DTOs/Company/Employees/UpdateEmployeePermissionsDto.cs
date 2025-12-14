using System.Collections.Generic;

namespace UserManagement.Application.DTOs.Company.Employees
{
    public class UpdateEmployeePermissionsDto
    {
        public string? AreaTrabajo { get; set; }
        public bool? EsSuperAdmin { get; set; }
        public Dictionary<string, ModuleAccessDto>? Permisos { get; set; }
    }
}

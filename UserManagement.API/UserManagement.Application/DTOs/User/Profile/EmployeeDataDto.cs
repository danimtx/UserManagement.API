using System.Collections.Generic;

namespace UserManagement.Application.DTOs.User.Profile
{
    public class EmployeeDataDto
    {
        public string EmpresaPadreId { get; set; } = string.Empty;
        public string? AreaTrabajo { get; set; }
        public Dictionary<string, string> Permisos { get; set; } = new();
    }
}

using UserManagement.Domain.Enums;

namespace UserManagement.Application.DTOs.Company.Employees
{
    public class UpdateEmployeeStatusDto
    {
        public UserStatus NuevoEstado { get; set; }
    }
}

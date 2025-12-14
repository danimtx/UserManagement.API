namespace UserManagement.Application.DTOs.Company.Employees
{
    public class UpdateEmployeeProfileDto
    {
        public string? Nombres { get; set; }
        public string? ApellidoPaterno { get; set; }
        public string? ApellidoMaterno { get; set; }
        public string? CI { get; set; }
        public string? Celular { get; set; }
        public string? Direccion { get; set; }
    }
}

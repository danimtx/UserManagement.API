namespace UserManagement.Application.DTOs.User.Profile
{
    public class UserProfileDto
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string TipoUsuario { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string? FotoPerfilUrl { get; set; }
        public string? Biografia { get; set; }

        public PersonalDataDto? DatosPersonales { get; set; }
        public CompanyDataDto? DatosEmpresa { get; set; }
        public EmployeeDataDto? DatosEmpleado { get; set; }
    }
}

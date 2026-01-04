namespace UserManagement.Application.DTOs.Admin.Support
{
    public class AdminUserSearchResultDto
    {
        public string Id { get; set; } = string.Empty;
        public string IdentificadorPrincipal { get; set; } = string.Empty; // Email
        public string NombreDisplay { get; set; } = string.Empty; // Full Name or Razon Social
        public string TipoUsuario { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
    }
}

namespace UserManagement.Application.DTOs.Admin.Support
{
    public class AdminChangeStatusDto
    {
        public string NuevoEstado { get; set; } = string.Empty;
        public string? Motivo { get; set; }
    }
}

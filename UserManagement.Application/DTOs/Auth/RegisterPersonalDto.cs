using System;
using System.Collections.Generic;
using System.Text;

namespace UserManagement.Application.DTOs.Auth
{
    public class RegisterPersonalDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;

        public string Nombres { get; set; } = string.Empty;
        public string ApellidoPaterno { get; set; } = string.Empty;
        public string? ApellidoMaterno { get; set; }
        public string UserName { get; set; } = string.Empty;
        public DateTime FechaNacimiento { get; set; }
        public string CI { get; set; } = string.Empty;
        public string? Nit { get; set; }
        public string? Seprec { get; set; }

        public string Pais { get; set; } = "Bolivia";
        public string Departamento { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string Celular { get; set; } = string.Empty;

        public string Profesion { get; set; } = string.Empty;
        public string? LinkedInUrl { get; set; }
        public List<DocumentoDto> Documentos { get; set; } = new();
    }
}
using System;
using System.Collections.Generic;
using UserManagement.Application.DTOs.Auth; // For RepresentativeDto, SucursalDto
using UserManagement.Application.DTOs.Shared; // For DocumentoDto

namespace UserManagement.Application.DTOs.Admin.Support
{
    // Unmasked DTO for admin view
    public class AdminPersonalProfileDto
    {
        public string Nombres { get; set; } = string.Empty;
        public string ApellidoPaterno { get; set; } = string.Empty;
        public string ApellidoMaterno { get; set; } = string.Empty;
        public DateTime FechaNacimiento { get; set; }
        public string CI { get; set; } = string.Empty;
        public string? Nit { get; set; }
        public string Direccion { get; set; } = string.Empty;
        public string Celular { get; set; } = string.Empty;
        public string Profesion { get; set; } = string.Empty;
        public List<DocumentoDto> DocumentosSoporte { get; set; } = new();
    }
    
    // Unmasked DTO for admin view
    public class AdminCompanyProfileDto
    {
        public string RazonSocial { get; set; } = string.Empty;
        public string Nit { get; set; } = string.Empty;
        public RepresentativeDto Representante { get; set; } = new();
        public List<SucursalDto> Sucursales { get; set; } = new();
        public List<DocumentoDto> DocumentosLegales { get; set; } = new();
    }

    public class AdminUserDetailDto
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? UserName { get; set; }
        public string TipoUsuario { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public DateTime FechaRegistro { get; set; }
        
        public AdminPersonalProfileDto? DatosPersonales { get; set; }
        public AdminCompanyProfileDto? DatosEmpresa { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using UserManagement.Application.DTOs.Shared;

namespace UserManagement.Application.DTOs.Auth
{
    public class RegisterCompanyDto
    {
        public string EmailEmpresa { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;

        public string RazonSocial { get; set; } = string.Empty;
        public string TipoEmpresa { get; set; } = string.Empty;
        public string Nit { get; set; } = string.Empty;

        // Contacto Central
        public string TelefonoFijo { get; set; } = string.Empty;
        public string CelularCorporativo { get; set; } = string.Empty;

        public RepresentativeDto Representante { get; set; } = new();

        // LISTAS
        public List<SucursalDto> Sucursales { get; set; } = new();

        public List<DocumentoDto> DocumentosLegales { get; set; } = new();

        public List<string> ModulosSolicitados { get; set; } = new();
    }

    public class SucursalDto
    {
        public string Nombre { get; set; } = "Oficina Central";
        public string Direccion { get; set; } = string.Empty;
        public string Departamento { get; set; } = string.Empty;
        public string Pais { get; set; } = "Bolivia";
        public double Latitud { get; set; }
        public double Longitud { get; set; }
        public string? Telefono { get; set; }
        public List<string> ModulosAsociados { get; set; } = new();
    }

    public class RepresentativeDto
    {
        public string Nombres { get; set; } = string.Empty;
        public string ApellidoPaterno { get; set; } = string.Empty;
        public string ApellidoMaterno { get; set; } = string.Empty;
        public string CI { get; set; } = string.Empty;
        public DateTime FechaNacimiento { get; set; }
        public string DireccionDomicilio { get; set; } = string.Empty;
        public string EmailPersonal { get; set; } = string.Empty;
        public List<string> NumerosContacto { get; set; } = new();
    }
}
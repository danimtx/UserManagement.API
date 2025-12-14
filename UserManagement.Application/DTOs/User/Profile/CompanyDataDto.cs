using System.Collections.Generic;

namespace UserManagement.Application.DTOs.User.Profile
{
    public class PerfilComercialResumenDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public double Estrellas { get; set; }
    }

    public class SucursalProfileDto
    {
        public string Nombre { get; set; } = "Oficina Central";
        public string Direccion { get; set; } = string.Empty;
        public string Departamento { get; set; } = string.Empty;
        public string? Telefono { get; set; }
    }

    public class CompanyDataDto
    {
        public string RazonSocial { get; set; } = string.Empty;
        public string Nit { get; set; } = string.Empty; // Masked value
        public string? Telefonos { get; set; }
        public List<SucursalProfileDto> Sucursales { get; set; } = new();
        public List<PerfilComercialResumenDto> Perfiles { get; set; } = new();
    }
}

using System.Collections.Generic;
using UserManagement.Application.DTOs.Shared; // Added

namespace UserManagement.Application.DTOs.User.Profile
{
    public class TagResumenDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public double Estrellas { get; set; }
    }

    public class PersonalDataDto
    {
        public string NombreCompleto { get; set; } = string.Empty;
        public string CI { get; set; } = string.Empty; // Masked value
        public string? Celular { get; set; }
        public string? Direccion { get; set; }
        public GeoLocationDto? UbicacionLaboral { get; set; } // Changed to GeoLocationDto
        public List<TagResumenDto> Tags { get; set; } = new();
    }
}
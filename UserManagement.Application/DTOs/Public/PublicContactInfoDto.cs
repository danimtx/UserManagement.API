using UserManagement.Application.DTOs.Shared; // Changed

namespace UserManagement.Application.DTOs.Public
{
    public class PublicContactInfoDto
    {
        public string Direccion { get; set; } = string.Empty;
        public GeoLocationDto? Ubicacion { get; set; } // Changed to GeoLocationDto
        public string? Telefono { get; set; }
    }
}
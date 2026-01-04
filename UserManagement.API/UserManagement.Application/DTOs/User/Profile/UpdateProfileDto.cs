using UserManagement.Application.DTOs.Shared; // Added

namespace UserManagement.Application.DTOs.User.Profile
{
    public class UpdateProfileDto
    {
        public string? FotoPerfilUrl { get; set; }
        public string? Biografia { get; set; }
        public string? Celular { get; set; }
        public string? Direccion { get; set; }
        public GeoLocationDto? UbicacionLaboral { get; set; } // Changed to GeoLocationDto
    }
}
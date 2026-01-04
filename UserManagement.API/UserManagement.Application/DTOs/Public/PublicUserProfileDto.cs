using System.Collections.Generic;

namespace UserManagement.Application.DTOs.Public
{
    public class PublicUserProfileDto
    {
        public string Id { get; set; } = string.Empty;
        public string TipoUsuario { get; set; } = string.Empty;
        public string NombreMostrar { get; set; } = string.Empty;
        public string? FotoUrl { get; set; }
        public string? Biografia { get; set; }
        public string? Ciudad { get; set; }
        public bool EsVerificado { get; set; }
        public PublicContactInfoDto? Contacto { get; set; }
        public List<PublicTagDto> Etiquetas { get; set; } = new();
    }
}

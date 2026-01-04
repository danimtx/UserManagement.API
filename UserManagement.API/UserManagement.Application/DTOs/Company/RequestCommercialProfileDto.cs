using System.Collections.Generic;
using UserManagement.Application.DTOs.Shared;

namespace UserManagement.Application.DTOs.Company
{
    public class RequestCommercialProfileDto
    {
        public string NombreComercial { get; set; } = string.Empty;
        public string? ModuloAsociado { get; set; } // Si es null, es "Tag Social"
        public string? Rubro { get; set; } // Solo para "Tag Social"
        public string? LogoUrl { get; set; }
        public List<DocumentoDto> Documentos { get; set; } = new List<DocumentoDto>();
    }
}

using System.Collections.Generic;
using UserManagement.Application.DTOs.Shared;

namespace UserManagement.Application.DTOs.User
{
    public class RequestPersonalModuleDto
    {
        public string NombreModulo { get; set; } = string.Empty;
        public List<DocumentoDto> DocumentosEvidencia { get; set; } = new();
    }
}

using System.Collections.Generic;
using UserManagement.Application.DTOs.Shared;

namespace UserManagement.Application.DTOs.User
{
    public class RequestTagDto
    {
        public string TagNombre { get; set; } = string.Empty;
        public bool EsEmpirico { get; set; }
        public List<DocumentoDto> Evidencias { get; set; } = new List<DocumentoDto>();
    }
}

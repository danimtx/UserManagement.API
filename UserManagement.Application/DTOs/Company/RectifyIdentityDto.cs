using System.Collections.Generic;
using UserManagement.Application.DTOs.Shared;

namespace UserManagement.Application.DTOs.Company
{
    public class RectifyIdentityDto
    {
        public string RazonSocial { get; set; } = string.Empty;
        public string Nit { get; set; } = string.Empty;
        // Allows user to re-upload legal documents if they were the cause of rejection
        public List<DocumentoDto> DocumentosLegales { get; set; } = new List<DocumentoDto>();
    }
}

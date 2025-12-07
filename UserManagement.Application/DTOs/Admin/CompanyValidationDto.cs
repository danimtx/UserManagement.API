using System;
using System.Collections.Generic;
using System.Text;
using UserManagement.Domain.Entities.ValueObjects;

namespace UserManagement.Application.DTOs.Admin
{
    public class CompanyValidationDto
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string RazonSocial { get; set; } = string.Empty;
        public string Nit { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public List<UploadedDocument> DocumentosLegales { get; set; } = new();
        public List<ModuleRequest> SolicitudesModulos { get; set; } = new();

        public string NombreRepresentante { get; set; } = string.Empty;
        public DateTime FechaRegistro { get; set; }
    }
}

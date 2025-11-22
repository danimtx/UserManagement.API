using System;
using System.Collections.Generic;
using System.Text;

namespace UserManagement.Application.DTOs.Admin
{
    public class CompanyValidationDto
    {
        public string Id { get; set; } = string.Empty; // El UID para saber a quién aprobar
        public string Email { get; set; } = string.Empty;
        public string RazonSocial { get; set; } = string.Empty;
        public string Nit { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;

        // Para que el Admin pueda hacer clic y ver los PDFs
        public Dictionary<string, string> DocumentosLegales { get; set; } = new();

        public string NombreRepresentante { get; set; } = string.Empty;
        public DateTime FechaRegistro { get; set; }
    }
}

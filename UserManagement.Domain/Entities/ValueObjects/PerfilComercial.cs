using System;
using System.Collections.Generic;
using UserManagement.Domain.Enums;

namespace UserManagement.Domain.Entities.ValueObjects
{
    public class PerfilComercial
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string NombreComercial { get; set; } = string.Empty;
        public CommercialProfileType Tipo { get; set; }
        public string? ModuloAsociado { get; set; }
        public string? LogoUrl { get; set; }
        public CommercialProfileStatus Estado { get; set; }
        public List<UploadedDocument> DocumentosEspecificos { get; set; } = new List<UploadedDocument>();
        
        // --- Campos de Reputación ---
        public double CalificacionPromedio { get; set; }
        public int TotalResenas { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace UserManagement.Domain.Entities.ValueObjects
{
    public class CompanyProfile
    {
        public string RazonSocial { get; set; } = string.Empty;
        public string TipoEmpresa { get; set; } = string.Empty;
        public string Nit { get; set; } = string.Empty;

        public LegalRepresentative Representante { get; set; } = new();

        public string TelefonoFijo { get; set; } = string.Empty;
        public string CelularCorporativo { get; set; } = string.Empty;

        // --- MULTI-UBICACIÓN ---
        public List<CompanyBranch> Sucursales { get; set; } = new();

        public List<UploadedDocument> DocumentosLegales { get; set; } = new();

        public List<string> AreasDefinidas { get; set; } = new List<string>() { "General" };

        // --- MULTI-CARA COMERCIAL ---
        public List<PerfilComercial> PerfilesComerciales { get; set; } = new();
    }

    public class LegalRepresentative
    {
        public string Nombres { get; set; } = string.Empty;
        public string ApellidoPaterno { get; set; } = string.Empty;
        public string ApellidoMaterno { get; set; } = string.Empty;
        public string CI { get; set; } = string.Empty;
        public DateTime FechaNacimiento { get; set; }
        public string DireccionDomicilio { get; set; } = string.Empty;
        public string EmailPersonal { get; set; } = string.Empty;
        public List<string> NumerosContacto { get; set; } = new();
    }
}
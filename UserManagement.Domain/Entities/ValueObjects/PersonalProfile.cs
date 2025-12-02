using System;
using System.Collections.Generic;
using System.Text;

namespace UserManagement.Domain.Entities.ValueObjects
{
    public class PersonalProfile
    {
        public string Nombres { get; set; } = string.Empty;
        public string ApellidoPaterno { get; set; } = string.Empty;
        public string ApellidoMaterno { get; set; } = string.Empty;
        public DateTime FechaNacimiento { get; set; }
        public string CI { get; set; } = string.Empty;
        public string? Nit { get; set; }
        public string? CodigoSeprec { get; set; }
        public string Pais { get; set; } = "Bolivia";
        public string Departamento { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string Celular { get; set; } = string.Empty;
        public string Profesion { get; set; } = string.Empty;
        public string? LinkedInUrl { get; set; }
        public bool VerificadoMarket { get; set; } = false;
        public List<UploadedDocument> DocumentosSoporte { get; set; } = new();
    }
}
using System;
using System.Collections.Generic;
using System.Text;

namespace UserManagement.Domain.Entities.ValueObjects
{
    public class GeoLocation
    {
        public double Lat { get; set; }
        public double Lng { get; set; }
    }

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

        // --- Nuevos Campos de Reputación y Visibilidad ---
        public string? Biografia { get; set; }
        public GeoLocation? UbicacionLaboral { get; set; }
        public bool DireccionVisible { get; set; } = false;
        public List<Tag> Tags { get; set; } = new List<Tag>();
    }
}
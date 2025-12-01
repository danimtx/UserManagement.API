using System;
using System.Collections.Generic;
using System.Text;

namespace UserManagement.Domain.Entities.ValueObjects
{
    public class CompanyBranch
    {
        public string NombreSucursal { get; set; } = string.Empty;
        public List<string> ModulosAsociados { get; set; } = new();

        // Dirección física
        public string DireccionEscrita { get; set; } = string.Empty;
        public string Departamento { get; set; } = string.Empty;
        public string Pais { get; set; } = "Bolivia";

        // Coordenadas Exactas
        public double Latitud { get; set; }
        public double Longitud { get; set; }

        public string? TelefonoSucursal { get; set; }

        public bool EsActiva { get; set; } = true;
    }
}

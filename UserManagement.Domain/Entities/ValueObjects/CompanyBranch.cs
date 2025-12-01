using System;
using System.Collections.Generic;
using System.Text;

namespace UserManagement.Domain.Entities.ValueObjects
{
    public class CompanyBranch
    {
        // Nombre interno para identificarla
        // Ej: "Oficina Central", "Depósito El Alto", "Sucursal Sur"
        public string NombreSucursal { get; set; } = string.Empty;

        // A qué módulo pertenece esta ubicación principal
        // Ej: Si es "Construccion", es una oficina. Si es "Ferreteria", es una tienda.
        // Puede ser una lista si la misma ubicación sirve para ambos.
        public List<string> ModulosAsociados { get; set; } = new();

        // Dirección física
        public string DireccionEscrita { get; set; } = string.Empty;
        public string Departamento { get; set; } = string.Empty;
        public string Pais { get; set; } = "Bolivia";

        // Coordenadas Exactas
        public double Latitud { get; set; }
        public double Longitud { get; set; }

        // Contacto específico de esa sucursal (opcional, si no usa el general)
        public string? TelefonoSucursal { get; set; }

        // Estado de la sucursal (Ej: "Abierta", "En Remodelación")
        public bool EsActiva { get; set; } = true;
    }
}

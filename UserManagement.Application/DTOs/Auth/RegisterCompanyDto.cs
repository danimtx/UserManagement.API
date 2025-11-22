using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Generic;

namespace UserManagement.Application.DTOs.Auth
{
    public class RegisterCompanyDto
    {
        // --- Credenciales de la Empresa ---
        public string EmailEmpresa { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;

        // --- Datos de la Empresa ---
        public string RazonSocial { get; set; } = string.Empty;
        public string TipoEmpresa { get; set; } = string.Empty; // SRL, SA...
        public string Nit { get; set; } = string.Empty;

        // --- Ubicación de la Empresa ---
        public string Pais { get; set; } = "Bolivia";
        public string Departamento { get; set; } = string.Empty;
        public string DireccionEscrita { get; set; } = string.Empty;
        // Coordenadas del mapa
        public double Latitud { get; set; }
        public double Longitud { get; set; }

        // --- Contactos Empresa ---
        public string TelefonoFijo { get; set; } = string.Empty;
        public string CelularCorporativo { get; set; } = string.Empty;

        // --- Representante Legal ---
        // Agrupamos estos datos para que sea más ordenado recibirlos
        public RepresentativeDto Representante { get; set; } = new();

        // --- Documentos Legales ---
        // Recibimos un diccionario con las URLs. Ej: {"nit": "https://...", "seprec": "https://..."}
        public Dictionary<string, string> DocumentosUrls { get; set; } = new();

        // --- Módulos Solicitados ---
        // Qué quiere contratar la empresa: ["Construccion", "Ferreteria"]
        public List<string> ModulosSolicitados { get; set; } = new();
    }

    public class RepresentativeDto
    {
        public string Nombres { get; set; } = string.Empty;
        public string ApellidoPaterno { get; set; } = string.Empty;
        public string ApellidoMaterno { get; set; } = string.Empty;
        public string CI { get; set; } = string.Empty;
        public string CIComplemento { get; set; } = string.Empty;
        public DateTime FechaNacimiento { get; set; }
        public string DireccionDomicilio { get; set; } = string.Empty;
        public string EmailPersonal { get; set; } = string.Empty;
        public List<string> NumerosContacto { get; set; } = new();
    }
}
using System;
using System.Collections.Generic;
using System.Text;

namespace UserManagement.Application.DTOs.Auth
{
    public class RegisterPersonalDto
    {
        // --- Credenciales de Acceso ---
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;

        // --- Datos Personales ---
        public string Nombres { get; set; } = string.Empty;
        public string ApellidoPaterno { get; set; } = string.Empty;
        public string? ApellidoMaterno { get; set; }

        public DateTime FechaNacimiento { get; set; }

        public string CI { get; set; } = string.Empty;
        public string CIComplemento { get; set; } = string.Empty;

        // --- Ubicación y Contacto ---
        public string Pais { get; set; } = "Bolivia";
        public string Departamento { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string Celular { get; set; } = string.Empty;

        // --- Datos Profesionales ---
        public string Profesion { get; set; } = string.Empty; // "Arquitecto", "Albañil"
        public string? LinkedInUrl { get; set; }
        public string? Nit { get; set; } // Opcional

        // --- URLs de Documentos (Subidos previamente al Storage desde el Frontend) ---
        public string FotoCiUrl { get; set; } = string.Empty;
        public string? FotoTituloUrl { get; set; } // Opcional
    }
}
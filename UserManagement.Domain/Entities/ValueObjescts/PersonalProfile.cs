using System;
using System.Collections.Generic;
using System.Text;
using Google.Cloud.Firestore;

namespace UserManagement.Domain.Entities.ValueObjects
{
    /// <summary>
    /// Contiene toda la información específica de una persona natural.
    /// No tiene ID propio porque vive dentro del documento del Usuario.
    /// </summary>
    [FirestoreData]
    public class PersonalProfile
    {
        // --- Datos de Identificación ---
        [FirestoreProperty]
        public string Nombres { get; set; } = string.Empty;
        [FirestoreProperty]
        public string ApellidoPaterno { get; set; } = string.Empty;
        [FirestoreProperty]
        public string ApellidoMaterno { get; set; } = string.Empty;
        [FirestoreProperty]
        public DateTime FechaNacimiento { get; set; }
        [FirestoreProperty]
        public string CI { get; set; } = string.Empty; //carnte de identiad  + complemento
        [FirestoreProperty]
        public string FotoCiUrl { get; set; } = string.Empty;


        // --- Ubicación y Contacto ---
        [FirestoreProperty]
        public string Pais { get; set; } = "Bolivia";
        [FirestoreProperty]
        public string Departamento { get; set; } = string.Empty;
        [FirestoreProperty]
        public string Direccion { get; set; } = string.Empty;
        [FirestoreProperty]
        public string Celular { get; set; } = string.Empty;
        [FirestoreProperty]
        public string? LinkedInUrl { get; set; }


        // --- Datos Profesionales (Opcionales según el oficio) ---
        [FirestoreProperty]
        public string Profesion { get; set; } = string.Empty; // Arquitecto, Albañil, etc.
        [FirestoreProperty]
        public string? FotoTituloUrl { get; set; }


        // --- Datos Fiscales (Opcional) ---
        [FirestoreProperty]
        public string? Nit { get; set; }
        [FirestoreProperty]
        public string? CodigoSeprec { get; set; }


        // --- Seguridad para MARKET ---
        // Por defecto nace en 'false'. El Admin lo cambia a 'true' tras revisar.
        [FirestoreProperty]
        public bool VerificadoMarket { get; set; } = false;
        [FirestoreProperty]
        public List<string> DocumentosValidacionMarket { get; set; } = new(); //lista de documentos cargados para validar al usuario en MARKET
    }
}
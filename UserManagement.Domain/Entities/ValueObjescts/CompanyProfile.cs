using System;
using System.Collections.Generic;
using System.Text;
using Google.Cloud.Firestore;

namespace UserManagement.Domain.Entities.ValueObjects
{
    /// <summary>
    /// Contiene los datos de la Empresa y de su Representante Legal.
    /// </summary>
    [FirestoreData]
    public class CompanyProfile
    {
        // --- Datos Generales de la Empresa ---
        [FirestoreProperty]
        public string RazonSocial { get; set; } = string.Empty;

        [FirestoreProperty]
        public string TipoEmpresa { get; set; } = string.Empty; // Ej: "S.R.L.", "Unipersonal", "S.A."

        [FirestoreProperty]
        public string Nit { get; set; } = string.Empty; // El número de NIT


        // --- Representante Legal (Objeto Anidado) ---
        // Aquí guardamos toda la data del dueño/representante
        [FirestoreProperty]
        public LegalRepresentative Representante { get; set; } = new();


        // --- Contacto y Ubicación de la OFICINA ---
        [FirestoreProperty]
        public string TelefonoFijo { get; set; } = string.Empty;

        [FirestoreProperty]
        public string CelularCorporativo { get; set; } = string.Empty;

        [FirestoreProperty]
        public string Pais { get; set; } = "Bolivia";

        [FirestoreProperty]
        public string Departamento { get; set; } = string.Empty; // Solo los 9 departamentos

        [FirestoreProperty]
        public string DireccionEscrita { get; set; } = string.Empty; // Texto descriptivo

        // Coordenadas para el mapa de Google (El punto en el mapa)
        [FirestoreProperty] public double Latitud { get; set; }
        [FirestoreProperty] public double Longitud { get; set; }


        // --- Documentos Legales (Archivos) ---
        // Usamos un Diccionario para flexibilidad. 
        // Clave (Key): "nit", "seprec", "constitucion", "poder", "licencia", "otro"
        // Valor (Value): La URL pública de la imagen/pdf en Firebase Storage
        [FirestoreProperty]
        public Dictionary<string, string> DocumentosLegales { get; set; } = new();
    }

    /// <summary>
    /// Clase auxiliar para agrupar datos del Representante.
    /// No es una entidad separada, vive dentro de CompanyProfile.
    /// </summary>
    [FirestoreData]
    public class LegalRepresentative
    {
        [FirestoreProperty] public string Nombres { get; set; } = string.Empty;
        [FirestoreProperty] public string ApellidoPaterno { get; set; } = string.Empty;
        [FirestoreProperty] public string ApellidoMaterno { get; set; } = string.Empty;

        [FirestoreProperty] public string CI { get; set; } = string.Empty;
        [FirestoreProperty] public string CIComplemento { get; set; } = string.Empty; // Por si tiene complemento (1A)

        [FirestoreProperty] public DateTime FechaNacimiento { get; set; }

        [FirestoreProperty] public string DireccionDomicilio { get; set; } = string.Empty;

        [FirestoreProperty] public string EmailPersonal { get; set; } = string.Empty;

        // "Contacto puede ser más de un número de celular" -> Usamos una lista
        [FirestoreProperty] public List<string> NumerosContacto { get; set; } = new();
    }
}
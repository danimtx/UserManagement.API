using System;
using System.Collections.Generic;
using System.Text;
using Google.Cloud.Firestore;

namespace UserManagement.Domain.Entities.ValueObjects
{
    //datos de la empresa y su representante legal
    [FirestoreData]
    public class CompanyProfile
    {
        [FirestoreProperty]
        public string RazonSocial { get; set; } = string.Empty;

        [FirestoreProperty]
        public string TipoEmpresa { get; set; } = string.Empty;

        [FirestoreProperty]
        public string Nit { get; set; } = string.Empty; // El número de NIT


        // --- Representante Legal
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
        public string Departamento { get; set; } = string.Empty;

        [FirestoreProperty]
        public string DireccionEscrita { get; set; } = string.Empty;

        // Coordenadas para el mapa de Google (El punto en el mapa)
        [FirestoreProperty] public double Latitud { get; set; }
        [FirestoreProperty] public double Longitud { get; set; }


        // --- Documentos Legales (Archivos) ---
        [FirestoreProperty]
        public Dictionary<string, string> DocumentosLegales { get; set; } = new();

        [FirestoreProperty]
        public List<string> AreasDefinidas { get; set; } = new List<string>() { "general" };
    }

    // clase aux para el representante legal de la empresa
    [FirestoreData]
    public class LegalRepresentative
    {
        [FirestoreProperty] public string Nombres { get; set; } = string.Empty;
        [FirestoreProperty] public string ApellidoPaterno { get; set; } = string.Empty;
        [FirestoreProperty] public string ApellidoMaterno { get; set; } = string.Empty;
        [FirestoreProperty] public string CI { get; set; } = string.Empty;
        [FirestoreProperty] public DateTime FechaNacimiento { get; set; }
        [FirestoreProperty] public string DireccionDomicilio { get; set; } = string.Empty;
        [FirestoreProperty] public string EmailPersonal { get; set; } = string.Empty;
        [FirestoreProperty] public List<string> NumerosContacto { get; set; } = new();
    }
}
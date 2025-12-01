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
        public string FotoCiUrl { get; set; } = string.Empty;

        // Datos de contacto personal (Del dueño)
        public string Pais { get; set; } = "Bolivia";
        public string Departamento { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty; // Dirección de casa
        public string Celular { get; set; } = string.Empty;

        public string Profesion { get; set; } = string.Empty;
        public string? FotoTituloUrl { get; set; }
        public string? LinkedInUrl { get; set; }
        public string? Nit { get; set; }

        // --- NUEVA LÓGICA DE VALIDACIÓN ---
        // Ya no es un simple booleano global. Ahora depende de los documentos.
        // Sin embargo, mantenemos este flag para búsquedas rápidas ("¿Puede vender?")
        public bool VerificadoMarket { get; set; } = false;

        // LISTA DE EVIDENCIAS: Aquí guardamos N documentos etiquetados
        public List<UploadedDocument> DocumentosSoporte { get; set; } = new();
    }
}
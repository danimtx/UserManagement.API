using System;
using System.Collections.Generic;
using System.Text;

namespace UserManagement.Domain.Entities.ValueObjects
{
    public class UploadedDocument
    {
        // Ej: "Factura de Luz", "Certificado de Antecedentes", "Foto Fachada"
        public string TipoDocumento { get; set; } = string.Empty;

        // URL del archivo en Firebase Storage
        public string UrlArchivo { get; set; } = string.Empty;

        // --- EL CAMBIO CLAVE ---
        // Para qué módulo sirve este documento.
        // Ej: "Market", "Construccion", "Ferreteria"
        public string ModuloObjetivo { get; set; } = string.Empty;

        // Estado individual de ESTE documento. 
        // Permite que el Agente de IA apruebe uno pero rechace otro (ej: foto borrosa).
        // Valores: "Pendiente", "Aprobado", "Rechazado"
        public string EstadoValidacion { get; set; } = "Pendiente";

        // Fecha de subida (útil para que el agente priorice los viejos)
        public DateTime FechaSubida { get; set; } = DateTime.UtcNow;

        // Comentario del revisor (IA o Admin) si se rechaza
        public string? MotivoRechazo { get; set; }
    }
}

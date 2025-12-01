using System;
using System.Collections.Generic;
using System.Text;

namespace UserManagement.Domain.Entities.ValueObjects
{
    public class UploadedDocument
    {
        public string TipoDocumento { get; set; } = string.Empty;
        public string UrlArchivo { get; set; } = string.Empty;
        public string ModuloObjetivo { get; set; } = string.Empty;
        public string EstadoValidacion { get; set; } = "Pendiente";
        public DateTime FechaSubida { get; set; } = DateTime.UtcNow;
        public string? MotivoRechazo { get; set; }
    }
}

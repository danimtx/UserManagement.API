using System;
using System.Collections.Generic;
using System.Text;

namespace UserManagement.Domain.Entities.ValueObjects
{
    public class ModuleRequest
    {
        public string NombreModulo { get; set; } = string.Empty;
        public string Estado { get; set; } = "Pendiente";
        public DateTime FechaSolicitud { get; set; } = DateTime.UtcNow;
        public string? MotivoRechazo { get; set; }
    }
}

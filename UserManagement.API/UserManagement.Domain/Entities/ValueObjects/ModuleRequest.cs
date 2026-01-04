using System;
using System.Collections.Generic;
using System.Text;
using UserManagement.Domain.Enums;

namespace UserManagement.Domain.Entities.ValueObjects
{
    public class ModuleRequest
    {
        public string NombreModulo { get; set; } = string.Empty;
        public ModuleRequestStatus Estado { get; set; } = ModuleRequestStatus.Pendiente;
        public string? MotivoRechazo { get; set; }
        public List<UploadedDocument> Evidencias { get; set; } = new();
    }
}

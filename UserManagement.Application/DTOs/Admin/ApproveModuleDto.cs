using System;
using System.Collections.Generic;
using System.Text;

namespace UserManagement.Application.DTOs.Admin
{
    public class ApproveModuleDto
    {
        public string CompanyId { get; set; } = string.Empty;
        public List<ModuleDecisionDto> Decisiones { get; set; } = new();
    }

    public class ModuleDecisionDto
    {
        public string NombreModulo { get; set; } = string.Empty;
        public bool Aprobar { get; set; }
        public string? MotivoRechazo { get; set; }
    }
}

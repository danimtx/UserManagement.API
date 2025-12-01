using System;
using System.Collections.Generic;
using System.Text;

namespace UserManagement.Domain.Entities.ValueObjects
{
    public class SubAccountPermissions
    {
        public string AreaTrabajo { get; set; } = string.Empty;
        public bool EsSuperAdminEmpresa { get; set; } = false;
        public Dictionary<string, ModuleAccess> Modulos { get; set; } = new();
    }

    public class ModuleAccess
    {
        public bool TieneAcceso { get; set; } = false;
        public List<string> FuncionalidadesPermitidas { get; set; } = new();
        public List<string> RecursosEspecificosAllowed { get; set; } = new();
    }
}

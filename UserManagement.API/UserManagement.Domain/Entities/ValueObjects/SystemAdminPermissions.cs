using System;
using System.Collections.Generic;
using System.Text;

namespace UserManagement.Domain.Entities.ValueObjects
{
    public class SystemAdminPermissions
    {
        public string RolSistema { get; set; } = string.Empty;
        public List<string> PermisosGlobales { get; set; } = new();
    }
}

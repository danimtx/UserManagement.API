using System;
using System.Collections.Generic;
using System.Text;

namespace UserManagement.Domain.Entities.ValueObjects
{
    public class SystemAdminPermissions
    {
        // Define el rol operativo del admin del sistema
        // Ej: "SoporteTecnico", "ValidadorDocumentos", "DBManager"
        public string RolSistema { get; set; } = string.Empty;

        // Lista de permisos especiales de alto nivel
        // Ej: "ResetearPasswords", "AprobarEmpresas", "EditarPreciosConstruccion", "VerLogsGlobales"
        public List<string> PermisosGlobales { get; set; } = new();
    }
}

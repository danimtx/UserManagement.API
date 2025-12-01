using System;
using System.Collections.Generic;
using System.Text;

namespace UserManagement.Domain.Entities.ValueObjects
{
    public class SubAccountPermissions
    {
        public string AreaTrabajo { get; set; } = string.Empty; // Ej: "Marketing"

        // Si es true, tiene acceso TOTAL a todo lo que tenga la empresa, 
        // excepto borrar al dueño. Ignora la lista 'Modulos'.
        public bool EsSuperAdminEmpresa { get; set; } = false;

        // Permisos granulares (Si no es SuperAdmin)
        public Dictionary<string, ModuleAccess> Modulos { get; set; } = new();
    }

    public class ModuleAccess
    {
        public bool TieneAcceso { get; set; } = false;

        // Qué puede hacer dentro (Ej: "CrearObra", "VerPresupuesto")
        // Aquí controlarás si pueden "Gestionar Usuarios" o no.
        public List<string> FuncionalidadesPermitidas { get; set; } = new();

        // A qué recursos específicos accede (Ej: "ID_Pagina_Facebook_1")
        public List<string> RecursosEspecificosAllowed { get; set; } = new();
    }
}

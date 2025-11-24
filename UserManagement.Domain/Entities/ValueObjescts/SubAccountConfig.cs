using System;
using System.Collections.Generic;
using System.Text;
using Google.Cloud.Firestore;

namespace UserManagement.Domain.Entities.ValueObjects
{
    // definicion de permisos y roles para sub-cuentas (empleados)
    [FirestoreData]
    public class SubAccountPermissions
    {
        // Clave (Key): Nombre del Módulo (ej: "Construccion", "Ferreteria", "Social")
        // Valor (Value): Objeto con el detalle de permisos
        [FirestoreProperty]
        public Dictionary<string, ModuleAccess> Modulos { get; set; } = new();
    }

    [FirestoreData]
    public class ModuleAccess
    {
        // ¿Puede entrar al módulo? 
        // Si es false, ni siquiera le aparece en el menú.
        [FirestoreProperty]
        public bool TieneAcceso { get; set; } = false;
        // Lista de funcionalidades permitidas DENTRO del módulo.
        // Ej: En "Construccion" -> ["Presobras", "Proveedores"] (Pero NO "Contabilidad")
        [FirestoreProperty]
        public List<string> FuncionalidadesPermitidas { get; set; } = new();

        // Lista de IDs de recursos específicos (Para el caso Social)
        // Ej: En "Social" -> ["ID_Perfil_Ferreteria"] (Pero NO el de Construcción)
        [FirestoreProperty]
        public List<string> RecursosEspecificosAllowed { get; set; } = new();

        //// Roles funcionales (ej: "ResidenteObra", "Contador", "CommunityManager")
        //[FirestoreProperty]
        //public List<string> RolesAsignados { get; set; } = new();

        //// Permisos atómicos para acciones finas (ej: "ver_presupuesto", "aprobar_gasto")
        //[FirestoreProperty]
        //public List<string> PermisosEspecificos { get; set; } = new();

        //// --- REQUISITO CLAVE ---
        //// Mencionaste: "podrá seleccionar si darle permiso una cuenta de la red social o a ambas".
        //// Aquí guardamos los IDs específicos a los que tiene permiso.
        //// Ej: En módulo Social -> ["id_perfil_ferreteria"] (y no ve el de construcción)
        //// Ej: En módulo Construcción -> ["id_proyecto_edificio_A"] (y no ve el Edificio B)
        //[FirestoreProperty]
        //public List<string> IdsRecursosPermitidos { get; set; } = new();
    }
}